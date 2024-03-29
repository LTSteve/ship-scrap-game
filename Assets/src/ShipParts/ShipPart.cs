using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.VFX;

public class ShipPart : MonoBehaviour, ITreeNode
{
    private Transform buildPoints;

    //Tree Properties
    [SerializeField]
    private ShipPart parent;
    [SerializeField]
    public List<ShipPart> children = new List<ShipPart>();

    public ShipPart Parent { get { return parent; } set { parent = value; } }
    public List<ShipPart> Children { get { return children; } set { children = value; } }

    public Ship MyShip;

    public string Name = "Component";

    public float Mass = 1f;
    public int Price = 10;
    public float Health = 1f;

    private float currentHealth;

    [SerializeField]
    private string prefabPath;

    private Transform scrapPrefab;

    private TemporaryEffect boomPrefab;

    private void Awake()
    {
        currentHealth = Health;

        buildPoints = transform.Find("connectionpoints");
        scrapPrefab = (Transform)Resources.Load("Scrap", typeof(Transform));
        boomPrefab = (TemporaryEffect)Resources.Load("ShipBoom", typeof(TemporaryEffect));
    }

    public Transform GetNearestBuildPoint(Vector3 point)
    {
        var nearestDistance = float.MaxValue;
        Transform nearest = null;

        foreach (Transform buildPoint in buildPoints)
        {
            var nextdist = Vector3.Distance(buildPoint.position, point);
            if (nextdist < nearestDistance)
            {
                nearestDistance = nextdist;
                nearest = buildPoint;
            }
        }

        return nearest;
    }

    public Transform GetBuildPoint(int index)
    {
        if (index < 0) { return null; }

        return buildPoints.GetChild(index % buildPoints.childCount);
    }

    public void ApplyShipStats(ShipState shipState)
    {
        var workingCenterOfMass = shipState.CenterOfMass * shipState.Mass + transform.localPosition * Mass;

        shipState.Mass += Mass;

        shipState.CenterOfMass = workingCenterOfMass / shipState.Mass;

        gameObject.SendMessage("ApplyShipStatsFromComponent", shipState, SendMessageOptions.DontRequireReceiver);
    }

    public void Explode(float force = 1f, bool fullRefund = false)
    {
        transform.Find("collider").gameObject.SetActive(false);

        var workingValue = fullRefund ? Price : Mathf.Clamp((int)(Price * 0.5f), 0, int.MaxValue);
        var maxIterations = Mathf.Clamp((int)Mathf.Sqrt(workingValue), 5, 100);
        var iterations = 0;
        while (workingValue > 5)
        {
            int nextPiece = 0;

            if (workingValue > 125 && Random.value > 0.25f)
            {
                nextPiece = Random.Range(100, workingValue);
            }
            else if (workingValue > 50 && Random.value > 0.25f)
            {
                nextPiece = Random.Range(50, workingValue);
            }
            else if (workingValue > 10 && Random.value > 0.25f)
            {
                nextPiece = Random.Range(10, workingValue);
            }
            else
            {
                nextPiece = Random.Range(1, workingValue);
            }

            _spawnScrap(force, nextPiece);
            workingValue -= nextPiece;

            iterations++;
            if (iterations > maxIterations)
            {
                break;
            }
        }

        if (workingValue > 0)
        {
            _spawnScrap(0f, workingValue);
        }

        var explosion = Instantiate(boomPrefab, transform.position, Quaternion.identity);
        explosion.OnDestroy = _onDestroy;

        IEnumerator _onDestroy(GameObject go)
        {
            var visualeffect = go.GetComponent<VisualEffect>();

            visualeffect.Stop();

            while (visualeffect.aliveParticleCount > 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        Destroy(this.gameObject);
    }

    public class ShipComponentData
    {
        public string Label;
        public string Value;
    }

    public List<ShipComponentData> GetData()
    {
        var datas = new List<ShipComponentData>();

        datas.Add(new ShipComponentData { Label = "C", Value = "" + Price });
        datas.Add(new ShipComponentData { Label = "W", Value = "" + Mass });
        datas.Add(new ShipComponentData { Label = "HP", Value = "" + Health });

        gameObject.SendMessage("GetDataFromComponent", datas, SendMessageOptions.DontRequireReceiver);

        return datas;
    }

    public void LoadPropertiesFromModel(ShipPartModel model)
    {
        prefabPath = model.PrefabLocation;

        transform.localPosition = model.Offset;
        transform.localRotation = model.Rotation;

        if(!Application.isEditor)
            gameObject.SendMessage("LoadComponentPropertiesFromModel", model, SendMessageOptions.DontRequireReceiver);
    }

    public ShipPartModel SavePropertiesToModel()
    {
        var model = new ShipPartModel();

        model.PrefabLocation = prefabPath;

        model.Offset = transform.localPosition;
        model.Rotation = transform.localRotation;

        if (!Application.isEditor)
            gameObject.SendMessage("SaveComponentPropertiesToModel", model, SendMessageOptions.DontRequireReceiver);

        return model;
    }

    public string GetTreeAddress()
    {
        var workingAddress = "";

        var workingParent = Parent;

        var workingChild = this;

        while (workingParent != null)
        {
            workingAddress = "" + workingParent.Children.IndexOf(workingChild) + workingAddress;
            workingChild = workingParent;
            workingParent = workingParent.Parent;
        }

        return workingAddress;
    }

    private void _spawnScrap(float velocity, int value)
    {
        Instantiate(scrapPrefab, transform.position, Quaternion.identity);
    }

    public ShipPart GetParent()
    {
        return Parent;
    }

    public void SetParent(ShipPart parent)
    {
        Parent = parent;
    }

    public List<ShipPart> GetChildren()
    {
        return Children;
    }

    public void SetChildren(List<ShipPart> children)
    {
        Children = children;
    }

    public void DrainHealth(float toDrain)
    {
        currentHealth -= toDrain;

        Debug.Log("Loosing Health! " + currentHealth);
        if (currentHealth <= 0)
        {
            _splitShipAtMe();
        }
    }

    private void _splitShipAtMe()
    {
        var amRoot = parent == null;

        if (!amRoot)
        {
            //decouple me from parent
            parent.children.Remove(this);
        }

        if (children.Count == 0)
        {
            //leaf nodes can simply be removed
            Explode();
            return;
        }

        var resultantWreckage = new List<ITreeNode[]>();

        //if i'm a branch node i need to add my parent's tree to the wreckage
        if (!amRoot)
        {
            resultantWreckage.Add(Maths.CreateTreeNodeList(MyShip.ShipRoot));
        }

        //add all children's trees to the wreckage
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];

            resultantWreckage.Add(Maths.CreateTreeNodeList(child));
            //deparent my children
            child.parent = null;
        }

        //build a list of properties to help decide which wreck is my new ship
        var shipScores = new List<ShipScore>();
        for(var i = 0; i < resultantWreckage.Count; i++)
        {
            var score = new ShipScore();
            score.WasRoot = amRoot && i == 0;
            score.WreckageIndex = i;

            foreach (var part in resultantWreckage[i])
            {
                score.PartCount++;
                if(part is Bridge)
                {
                    score.BridgeCount++;
                }
            }

            shipScores.Add(score);
        }

        ITreeNode[] newShip = null;

        //first try to get a wreck with a bridge, prioritizing one that contained the previous root if possible
        var wrecksWithBridges = shipScores.Where(x => x.BridgeCount > 0);
        if(wrecksWithBridges.Count() != 0)
        {
            if (wrecksWithBridges.Any(x => x.WasRoot))
            {
                newShip = resultantWreckage[wrecksWithBridges.First(x => x.WasRoot).WreckageIndex];
            }
            else
            {
                newShip = resultantWreckage[wrecksWithBridges.OrderBy(x => x.PartCount).First().WreckageIndex];
            }
        }

        //next just assign whichever was the root or has the most parts
        if(newShip == null)
        {
            if (amRoot)
            {
                newShip = resultantWreckage[shipScores.OrderBy(x => x.PartCount).First().WreckageIndex];
            }
            else
            {
                newShip = resultantWreckage[shipScores.First(x => x.WasRoot).WreckageIndex];
            }
        }

        //finally, remove the new ship parts from the wreckage
        resultantWreckage.Remove(newShip);

        var wreckagePrefab = (Transform)Resources.Load("Wreckage", typeof(Transform));

        //spawn wrecks with the resultant wreckage parts
        foreach(var wreckagePartsList in resultantWreckage)
        {
            var firstShipComponent = (ShipPart)wreckagePartsList[0];
            var newWreckageTransform = Instantiate(wreckagePrefab, firstShipComponent.transform.position, firstShipComponent.transform.rotation);
            var newWreckage = newWreckageTransform.GetComponent<Wreckage>();

            newWreckage.ShipRoot = firstShipComponent;
            var newWreckageModel = newWreckage.transform.Find("Model");

            foreach(var part in wreckagePartsList)
            {
                var shipComponent = (ShipPart)part;
                shipComponent.transform.parent = newWreckageModel;
                shipComponent.MyShip = newWreckage;
            }

            //add some force to push the new bits away from eachother
            newWreckage.GetComponent<Rigidbody>().AddExplosionForce(1f, transform.position, 5f, 0f, ForceMode.Impulse);
        }

        //set new root
        var newRoot = (ShipPart)newShip[0];
        MyShip.ShipRoot = newRoot;

        //add some force to push the new bits away from eachother
        MyShip.GetComponent<Rigidbody>().AddExplosionForce(1f, transform.position, 5f, 0f, ForceMode.Impulse);

        //my job here is done
        Explode(1f, false);

    }

    private class ShipScore{
        public bool WasRoot;
        public int PartCount;
        public int BridgeCount;
        public int WreckageIndex;
    }
}
