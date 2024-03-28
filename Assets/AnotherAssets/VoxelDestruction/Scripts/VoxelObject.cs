using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using VoxReader.Interfaces;
using Random = UnityEngine.Random;

namespace VoxelDestruction
{
    public class VoxelObject : MonoBehaviour
    {
        private const string ASSET_PATH = "Assets/RemoteModels/";

        #region Variables

        //Global

        public string path;
        public bool exportMesh = false;
        public bool startWithPhysics = false;
        public bool createCollider = true;
        public bool useConvexCollider = false;
        public bool addVoxelCollider = false;
        public float collisionScale = 1;
        public bool buildEntire = true;
        public int buildModel = 0;

        //General

        public float objectScale = 1;

        public PivotType pivotType;
        public enum PivotType
        {
            Standard, Center, Min
        }

        public int loadingTime = 1;
        public Material material;
        public bool delayRecalculation = true;
        public bool isResetable = false;
        public bool use32BitInt = false;
        public GenerationType generationType = GenerationType.Safe;
        public enum GenerationType
        {
            Normal, Safe, Fast
        }

        public bool scheduleParallel = false;

        public Allocator allocator = Allocator.Persistent;

        //Collapsing

        public bool collapse;
        public bool physicsOnCollapse;
        public int collapsePhysicsLimit;

        //Physics

        public float totalMass = 10;
        public float drag = 0;
        public float angularDrag = 0.05f;

        public RigidbodyConstraints constraints = RigidbodyConstraints.None;
        public RigidbodyInterpolation interpolation = RigidbodyInterpolation.Interpolate;
        public CollisionDetectionMode collisionMode = CollisionDetectionMode.Discrete;

        //Destruction Settings

        public bool destructible = false;

        public DestructionType destructionType = DestructionType.PreCalculatedFragments;
        public enum DestructionType
        {
            //Abbreviations: PVR, AVR, FR and PF
            PassiveVoxelRemoval, ActiveVoxelRemoval, FragmentationRemoval, PreCalculatedFragments
        }

        //only FR
        public bool relativeSize = false;


        //Only PVR and AVR
        public GameObject particle = null;

        //PVR, AVR, FR and PF
        public DestructionGeneralSettings destructionGS;
        //PVR, AVR, FR and PF
        public DestructionSoundSettings destructionSS;

        //FR not relative
        public DestructionFragmentNRSettings destructionFNRS;
        //FR relative
        public DestructionFragmentRSettings destructionFRS;
        //PF
        public DestructionPreFragmentSettings destructionPFS;

        //Debug

        public bool debug;

        //Events

        public bool destroyWhenFullyDestroyed = true;
        public bool destroyDestructionPercentage = false;
        private float destructionPercentage = 0;
        private int initialVoxelCount;

        public bool isMeshBuilt { get; private set; } = false;

        //Parameters: position, strength
        public UnityEvent<Vector3, float> onDestruction;
        public UnityEvent onFullyDestructed;
        //Parameters: position, strength, percentage
        public UnityEvent<Vector3, float, float> onDestructionPercentage;

        public event Action OnMeshBuilt;

        //Private
        //Not in inspector
        public bool notBuildOnStart = false;

        private MeshFilter meshFiler;

        private Transform fragmentParent;

        private VoxelData data;
        private Vector3Int length;
        private VoxelData dataCopy;

        private bool activeRb;

        private int4 arrayLength;

        private List<GameObject> activateOnRecalculation;

        private int destructionCount;
        private Coroutine recalculationI;

        private FragmentBuilder fragmentBuilder;
        private Transform[] fragmentsT;

        private CollapseBuilder collapseBuilder;
        private bool needsCollapsing;
        private bool activeCollapsing;

#if UNITY_EDITOR
        //Only for CustomEditor
        public bool autoReload;
#endif

        private bool pivotSet;
        private Vector3 pivotOffset;

        #endregion

        //This region contains all code necesarry for initializing the voxel object
        #region ObjectCreation



        //Destroy editor preview mesh if Exists
        private void Awake()
        {
#if UNITY_EDITOR
            if (transform.Find("EDITOR_DEMO_MESH"))
            {
                DestroyImmediate(transform.Find("EDITOR_DEMO_MESH").gameObject);
            }
#endif
            this.gameObject.tag = "VoxelObject";
        }
        private void Start()
        {
            /*
            This only gets called on the first layer of the voxel object, 
            the next ones get instantiated and build manually
            */
            if (!notBuildOnStart)
            {
                destructionCount = 0;

#if !UNITY_WEBGL
                BetterStreamingAssets.Initialize();
                if (BetterStreamingAssets.FileExists(path))
                {
#endif
                if (debug)
                    Debug.Log("Reading Model...", this);

#if UNITY_WEBGL
                StartCoroutine(LoadAssetCoroutine(path, DrawVoxelFile));
                //LoadAsset(path, DrawVoxelFile);
#else
                    IVoxFile file = VoxReader.VoxReader.Read(path, true);
                    
                    if (file == null)
                        return;
                    
                    DrawVoxelFile(file);
#endif
#if !UNITY_WEBGL
                }
                else
                {
                    Debug.LogWarning("File with path " + path + " not found! \n Checked path: " + path, this);
                }
#endif
            }
        }

        private void OnEnable()
        {
            VoxelObjectManager.Instance.AddVoxelObject(this);
        }

        private void OnDisable()
        {
            VoxelObjectManager.Instance.RemoveVoxelObject(this);
        }

        /*
        This method creates all necessary objects for the voxel object,
        depening if all layers should get build it instantiated layers into new 
        objects. 
        */
        private void DrawVoxelFile(IVoxFile file)
        {
            if (buildEntire)
            {
                for (int i = 0; i < file.Models.Length; i++)
                {
                    if (debug)
                        Debug.Log("Drawing Model " + i + "...", this);

                    if (i == 0)
                    {
                        GameObject modelChild = new GameObject("VoxelModel " + i);
                        modelChild.transform.parent = transform;
                        modelChild.transform.localPosition = new Vector3(-0.5f, 0.5f, 0.5f);
                        modelChild.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        modelChild.transform.localScale = Vector3.one;
                        meshFiler = modelChild.AddComponent<MeshFilter>();

                        modelChild.AddComponent<MeshRenderer>();
                        modelChild.GetComponent<MeshRenderer>().material = material;
                        //This shadow casting mode works better with Voxels, but feel free to change it
                        modelChild.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.TwoSided;

                        transform.localScale = new Vector3(objectScale, objectScale, objectScale);

                        BuildModel(file.Models[i], i);
                    }
                    else
                    {
                        GameObject newParent = new GameObject(gameObject.name + " " + (i + 1).ToString());
                        newParent.transform.position = transform.position;
                        newParent.transform.rotation = transform.rotation;
                        newParent.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
                        newParent.transform.parent = transform.parent;

                        VoxelObject other = newParent.AddComponent<VoxelObject>();
                        other.notBuildOnStart = true;
                        other.debug = debug;
                        other.material = material;

                        other.destructible = destructible;
                        other.pivotType = pivotType;

                        other.destructionGS = destructionGS;
                        other.destructionCount = destructionCount;
                        other.destructionSS = destructionSS;
                        other.destructionPFS = destructionPFS;
                        other.destructionFRS = destructionFRS;
                        other.destructionFNRS = destructionFNRS;
                        other.relativeSize = relativeSize;
                        other.particle = particle;
                        other.destructionType = destructionType;

                        other.totalMass = totalMass;
                        other.drag = drag;
                        other.angularDrag = angularDrag;
                        other.constraints = constraints;
                        other.interpolation = interpolation;
                        other.collisionMode = collisionMode;
                        other.useConvexCollider = useConvexCollider;

                        other.use32BitInt = use32BitInt;
                        other.generationType = generationType;
                        other.loadingTime = loadingTime;

                        other.isResetable = isResetable;
                        other.delayRecalculation = delayRecalculation;

                        other.collapse = collapse;
                        other.collapsePhysicsLimit = collapsePhysicsLimit;
                        other.physicsOnCollapse = physicsOnCollapse;

                        other.objectScale = objectScale;
                        other.startWithPhysics = startWithPhysics;
                        other.path = path;
                        other.exportMesh = exportMesh;
                        other.addVoxelCollider = addVoxelCollider;
                        other.collisionScale = collisionScale;

                        other.destroyWhenFullyDestroyed = destroyWhenFullyDestroyed;
                        other.destroyDestructionPercentage = destroyDestructionPercentage;
                        other.onFullyDestructed = onFullyDestructed;
                        other.onDestructionPercentage = onDestructionPercentage;
                        other.onDestruction = onDestruction;

                        GameObject modelChild = new GameObject("VoxelModel " + i);
                        modelChild.transform.parent = newParent.transform;
                        modelChild.transform.localPosition = new Vector3(-0.5f, 0.5f, 0.5f);
                        modelChild.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        modelChild.transform.localScale = Vector3.one;
                        other.meshFiler = modelChild.AddComponent<MeshFilter>();

                        modelChild.AddComponent<MeshRenderer>();
                        modelChild.GetComponent<MeshRenderer>().material = material;
                        modelChild.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.TwoSided;

                        other.BuildModel(file.Models[i], i);
                    }
                }
            }
            else if (buildModel < file.Models.Length)
            {
                GameObject modelChild = new GameObject("VoxelModel " + buildModel);
                modelChild.transform.parent = transform;
                modelChild.transform.localPosition = new Vector3(-0.5f, 0.5f, 0.5f);
                modelChild.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                modelChild.transform.localScale = Vector3.one;
                meshFiler = modelChild.AddComponent<MeshFilter>();

                modelChild.AddComponent<MeshRenderer>();
                modelChild.GetComponent<MeshRenderer>().material = material;
                modelChild.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.TwoSided;

                transform.localScale = new Vector3(objectScale, objectScale, objectScale);

                BuildModel(file.Models[buildModel], buildModel);
            }
        }

        /*
        This method initializes the Voxel Mesh and converts the saved ".vox" files into VoxelData
        */
        public void BuildModel(IModel model, int modelIndex)
        {
            activateOnRecalculation = new List<GameObject>();
            length = Vector3Int.RoundToInt(model.Size);

            /*
            This is a fix to a problem I encountered:
            IForJobs don't support Native Lists, this makes it difficult to save triangles and vertecies
            This "Arraylength" int4 describes a length of array that is surrley big enough to fit all Triangles, Vertices, ...
            I have not found a better way to do this without wasting allocation memory
            */
            arrayLength = new int4(length.x * length.y * length.z * 4 * 6, length.x * length.y * length.z * 2 * 6,
                length.x * length.y * length.z * 4 * 6, length.x * length.y * length.z * 4 * 6);
            arrayLength.xyzw /= 2;

            if (debug)
                Debug.Log("Read Model Dimensions: " + $"{length.x}/{length.y}/{length.z}", this);


            data = VoxToVoxelData.GenerateVoxelData(model);


            if (isResetable)
            {
                dataCopy = new VoxelData(length, new Voxel[data.Blocks.Length], data.PositionOffset);

                for (int i = 0; i < dataCopy.Blocks.Length; i++)
                {
                    dataCopy.Blocks[i] = new Voxel(data.Blocks[i]);
                }
            }

            if (destroyDestructionPercentage)
                initialVoxelCount = GetVoxelCount();

            //Check if sound Manager exists
            if (destructible && destructionSS.collisionClip.Length > 0 && SoundManager.instance == null)
            {
                destructionSS.collisionClip = new string[] { };
                Debug.LogWarning("You are using collision clips, but there is no Sound Manager in the scene!", this);
            }

            if (destructionType == DestructionType.PreCalculatedFragments && destructible)
                PrecalculateFragments(modelIndex);

#if !UNITY_WEBGL
            FinishModel();
#endif
        }

        /*
        This method simply finished the initialization started in "BuildModel", it is a seperate 
        method since WebGL needs to load load the fragments first
        */
        private void FinishModel()
        {
            if (debug)
                Debug.Log("Creating Mesh...", this);

            if (createCollider || startWithPhysics)
            {
                meshFiler.gameObject.AddComponent<MeshCollider>();
                meshFiler.GetComponent<MeshCollider>().cookingOptions = MeshColliderCookingOptions.UseFastMidphase;
                meshFiler.GetComponent<MeshCollider>().convex = useConvexCollider;
            }

            if (addVoxelCollider)
            {
                meshFiler.gameObject.AddComponent<VoxelCollider>();
                meshFiler.GetComponent<VoxelCollider>().collisionScale = collisionScale;
            }

            if (!startWithPhysics)
            {
                activeRb = false;
            }
            else
            {
                meshFiler.gameObject.GetComponent<MeshCollider>();
                meshFiler.GetComponent<MeshCollider>().convex = true;

                Rigidbody rb = meshFiler.gameObject.AddComponent<Rigidbody>();
                rb.mass = totalMass;
                rb.drag = drag;
                rb.angularDrag = angularDrag;

                rb.interpolation = interpolation;
                rb.collisionDetectionMode = collisionMode;
                rb.constraints = constraints;

                activeRb = true;
            }

            StartCoroutine(CreateVoxelMesh(meshFiler, true));
        }

        #endregion

        //This region contains the code for creating the jobs to calculate the mesh
        #region Recalculation

        //A reference needs to be saved so that they can be disposed globally
        private MeshBuilder builder;
        private MeshBuilderParallel parallelBuilder;
        private MeshBuilderSafe safeBuilder;

        //Main function that draws the voxel mesh
        private IEnumerator CreateVoxelMesh(MeshFilter filter, bool forceSafe)
        {
            if (generationType == GenerationType.Normal && !forceSafe)
            {
                builder = new MeshBuilder();
                builder.StartMeshDrawing(data, arrayLength, allocator, scheduleParallel);

                for (int i = 0; i < loadingTime; i++)
                {
                    if (!builder.IsCompleted())
                        yield return null;
                    else
                        break;
                }

                Mesh mesh = builder.GetVoxelObject(use32BitInt);

                builder.Dispose();
                builder = null;

                if (mesh.vertices.Length == 0 || mesh.triangles.Length == 0)
                {
                    if (onFullyDestructed != null)
                        onFullyDestructed.Invoke();

                    ForceJobDispose();

                    if (destroyWhenFullyDestroyed)
                        Destroy(gameObject);
                    else
                    {
                        destructible = false;
                        filter.mesh = null;
                        if (createCollider)
                            filter.GetComponent<MeshCollider>().sharedMesh = null;
                    }

                    yield break;
                }

                filter.mesh = mesh;

                ProcessPivot();

                if (createCollider)
                    filter.GetComponent<MeshCollider>().sharedMesh = mesh;

                if (debug)
                    Debug.Log($"Finished Building Mesh: verts {mesh.vertices.Length}, triangles {mesh.triangles.Length}, uvs {mesh.uv.Length}, color {mesh.colors.Length}", this);
            }
            else if (generationType == GenerationType.Safe || forceSafe)
            {
                safeBuilder = new MeshBuilderSafe();
                safeBuilder.StartMeshDrawing(data, allocator);

                for (int i = 0; i < loadingTime; i++)
                {
                    if (!safeBuilder.IsCompleted())
                        yield return null;
                    else
                        break;
                }

                Mesh mesh = safeBuilder.GetVoxelObject(use32BitInt);

                safeBuilder.Dispose();
                safeBuilder = null;

                if (mesh.vertices.Length == 0 || mesh.triangles.Length == 0)
                {
                    if (onFullyDestructed != null)
                        onFullyDestructed.Invoke();

                    ForceJobDispose();

                    if (destroyWhenFullyDestroyed)
                        Destroy(gameObject);
                    else
                    {
                        destructible = false;
                        filter.mesh = null;
                        if (createCollider)
                            filter.GetComponent<MeshCollider>().sharedMesh = null;
                    }

                    yield break;
                }

                filter.mesh = mesh;

                ProcessPivot();

                if (createCollider)
                    filter.GetComponent<MeshCollider>().sharedMesh = mesh;

                if (debug)
                    Debug.Log($"Finished Building Mesh: verts {mesh.vertices.Length}, triangles {mesh.triangles.Length}, uvs {mesh.uv.Length}, color {mesh.colors.Length}", this);
            }
            else if (generationType == GenerationType.Fast)
            {
                parallelBuilder = new MeshBuilderParallel();
                parallelBuilder.StartMeshDrawing(data, arrayLength, allocator, scheduleParallel);

                for (int i = 0; i < loadingTime; i++)
                {
                    if (!parallelBuilder.IsCompleted())
                        yield return null;
                    else
                        break;
                }

                Mesh mesh = parallelBuilder.GetVoxelObject(use32BitInt);

                parallelBuilder.Dispose();
                parallelBuilder = null;

                if (mesh.vertices.Length == 0 || mesh.triangles.Length == 0)
                {
                    if (onFullyDestructed != null)
                        onFullyDestructed.Invoke();

                    ForceJobDispose();

                    if (destroyWhenFullyDestroyed)
                        Destroy(gameObject);
                    else
                    {
                        destructible = false;
                        filter.mesh = null;
                        if (createCollider)
                            filter.GetComponent<MeshCollider>().sharedMesh = null;
                    }

                    yield break;
                }

                filter.mesh = mesh;

                ProcessPivot();

                if (createCollider)
                    filter.GetComponent<MeshCollider>().sharedMesh = mesh;

                if (debug)
                    Debug.Log($"Finished Building Mesh: verts {mesh.vertices.Length}, triangles {mesh.triangles.Length}, uvs {mesh.uv.Length}, color {mesh.colors.Length}", this);
            }

            if (delayRecalculation && activateOnRecalculation.Count > 0)
            {
                for (int i = 0; i < activateOnRecalculation.Count; i++)
                {
                    activateOnRecalculation[i].transform.root.gameObject.SetActive(true);
                }

                activateOnRecalculation.Clear();
            }

            isMeshBuilt = true;
            OnMeshBuilt?.Invoke();
        }

        //Collapsing is handeled in update for better performance
        private void Update()
        {
            if (collapse && needsCollapsing && !activeCollapsing && destructionCount == 0)
            {
                needsCollapsing = false;

                collapseBuilder = new CollapseBuilder(data.GetVoxels(), length);

                activeCollapsing = true;
            }

            if (activeCollapsing && collapseBuilder.IsCompleted())
            {
                int[] pieces = collapseBuilder.GetResult();

                if (debug)
                    Debug.Log("Collapsing job finished, result/original length: " + pieces.Length + ", " + data.GetVoxels().Count(i => i == 1), this);

                if (pieces.Contains(-1))
                {
                    //Remove pieces from Voxel Object

                    List<VoxReader.Voxel[]> frag = new List<VoxReader.Voxel[]>();
                    List<int[]> toRemove = new List<int[]>();
                    List<VoxReader.Voxel> currentSubArray = new List<VoxReader.Voxel>();
                    List<int> currentToRemove = new List<int>();

                    for (int i = 0; i < pieces.Length; i++)
                    {
                        if (pieces[i] == -1)
                        {
                            frag.Add(currentSubArray.ToArray());
                            toRemove.Add(currentToRemove.ToArray());
                            currentSubArray.Clear();
                            currentToRemove.Clear();
                        }
                        else
                        {
                            currentSubArray.Add(new VoxReader.Voxel(To3D(pieces[i], length.x, length.y), new Color(data.Blocks[pieces[i]].color.r * 255, data.Blocks[pieces[i]].color.g * 255, data.Blocks[pieces[i]].color.b * 255, 255)));
                            currentToRemove.Add(pieces[i]);
                        }
                    }

                    if (currentSubArray.Count > 0)
                    {
                        frag.Add(currentSubArray.ToArray());
                        toRemove.Add(currentToRemove.ToArray());
                    }

                    int targetOriginal = 0;
                    int cOrg = frag[0].Length;
                    for (int i = 1; i < frag.Count; i++)
                    {
                        if (frag[i].Length > cOrg)
                        {
                            targetOriginal = i;
                            cOrg = frag[i].Length;
                        }
                    }

                    for (int i = 0; i < frag.Count; i++)
                    {
                        if (i == targetOriginal)
                            continue;

                        for (int j = 0; j < toRemove[i].Length; j++)
                            data.Blocks[toRemove[i][j]] = new Voxel(Color.black, false);

                        if (!activeRb && physicsOnCollapse && toRemove[i].Length > collapsePhysicsLimit)
                        {
                            activeRb = true;

                            meshFiler.GetComponent<MeshCollider>().convex = true;

                            Rigidbody myRb = meshFiler.gameObject.AddComponent<Rigidbody>();

                            myRb.mass = totalMass;
                            myRb.drag = drag;
                            myRb.angularDrag = angularDrag;
                            myRb.constraints = constraints;
                            myRb.interpolation = interpolation;
                            myRb.collisionDetectionMode = collisionMode;
                        }

                        if (debug)
                            Debug.Log("Removed " + toRemove[i].Length + " voxels using collapsing", this);

                        GameObject newCube = new GameObject();
                        newCube.transform.position =
                            transform.GetChild(0).TransformPoint(GetMinV3(frag[i]));

                        newCube.transform.name = transform.name + " Fragment Collapsed";

                        newCube.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

                        newCube.AddComponent<VoxelStruct>();
                        newCube.GetComponent<VoxelStruct>().BuildObject(frag[i], material, allocator, delayRecalculation, isResetable ? this : null);

                        Rigidbody cube = newCube.AddComponent<Rigidbody>();

                        float cubeMass = (totalMass / (length.x * length.y * length.z)) * frag[i].Length;

                        cube.mass = cubeMass;
                        cube.drag = drag;
                        cube.angularDrag = angularDrag;
                        cube.interpolation = interpolation;
                    }

                    QuitRecalculation();

                    recalculationI = StartCoroutine(CreateVoxelMesh(meshFiler, false));
                }

                collapseBuilder.Dispose();

                activeCollapsing = false;
            }
        }

        //Method to quit the recalculation process
        private void QuitRecalculation()
        {
            if (recalculationI != null)
            {
                StopCoroutine(recalculationI);

                ForceJobDispose();
            }
        }

        #endregion

        //Basic Destruction Methods
        #region Collision

        //Collision between Voxel Objects
        public void OnVoxelCollision(Collision collision, float _collisionScale)
        {
            if (!destructible)
                return;

            ComputeCollision(collision.relativeVelocity.magnitude * _collisionScale, collision.contacts[0].point, collision.contacts[0].normal, -1f);
        }

        /*
        Public destruction event, one option with and one without overrideMax
        overrideMax: Overrides the maximum distance of voxels to the collision points
        */
        public void AddDestruction(float strength, Vector3 point, Vector3 normal, float overrideMax)
        {
            if (!destructible)
                return;

            ComputeCollision(strength, point, normal, overrideMax);
        }

        public void AddDestruction(float strength, Vector3 point, Vector3 normal)
        {
            if (!destructible)
                return;

            ComputeCollision(strength, point, normal, -1f);
        }


        #endregion

        //Destruction Logic
        #region Destruction

        //Starts the IEnumerator for destruction
        private void ComputeCollision(float relativeVel, Vector3 point, Vector3 normal, float overrideMax)
        {
            if (debug)
                Debug.Log($"Destruction entering with strength: {relativeVel}, point: {point}, normal: {normal}, overrideMax: {overrideMax}");

            if (relativeVel > destructionGS.minCollisionMag)
                StartCoroutine(CreateDestruction(relativeVel, point, normal, overrideMax));
        }

        //This method contains the destruction logic
        private IEnumerator CreateDestruction(float relativeVel, Vector3 point, Vector3 normal, float overrideMax)
        {
            //Saves the current count of "CreateDestruction" Coroutines
            destructionCount++;

            //Sound
            if (destructionSS.collisionClip != null && destructionSS.collisionClip.Length > 0)
            {
                string clip = destructionSS
                    .collisionClip[Random.Range(0, destructionSS.collisionClip.Length)];
                SoundManager.instance.Play(clip, "Game", true, point, true, Mathf.Clamp(relativeVel / destructionSS.soundVolumeScale, 0.1f, 2f));
            }

            //Active and Passive Voxel Destruction
            if (destructionType == DestructionType.PassiveVoxelRemoval ||
                destructionType == DestructionType.ActiveVoxelRemoval)
            {
                //Get point voxel

                Vector3 localPoint = meshFiler.transform.InverseTransformPoint(point);

                int removalCount = Mathf.FloorToInt(relativeVel * destructionGS.collisionStrength);

                Color col = Color.magenta;
                for (int j = 0; j < removalCount; j++)
                {
                    int voxel = GetNearestVoxel(localPoint, overrideMax == -1f ? destructionGS.maxVoxelDistance : overrideMax);

                    if (voxel == -1)
                        break;

                    col = data.Blocks[voxel].color;

                    if (destructionType ==
                        DestructionType.ActiveVoxelRemoval)
                    {
                        GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        newCube.transform.position =
                            transform.GetChild(0).TransformPoint(To3D(voxel, data.Size.x, data.Size.y));

                        Rigidbody cube = newCube.AddComponent<Rigidbody>();
                        newCube.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

                        float cubeMass = totalMass / (length.x * length.y * length.z);

                        cube.mass = cubeMass;
                        cube.drag = 0;
                        cube.angularDrag = 0.05f;
                        cube.interpolation = interpolation;

                        cube.AddForce(normal * destructionGS.collisionForce, ForceMode.Impulse);

                        Mesh mesh = cube.GetComponent<MeshFilter>().mesh;
                        Color[] colors = new Color[mesh.vertices.Length];

                        for (int k = 0; k < mesh.vertices.Length; k++)
                        {
                            colors[k] = col;
                        }

                        mesh.colors = colors;

                        cube.GetComponent<MeshFilter>().mesh = mesh;

                        cube.GetComponent<MeshRenderer>().material = material;

                        if (delayRecalculation)
                        {
                            newCube.SetActive(false);
                            activateOnRecalculation.Add(newCube);
                        }
                    }
                    data.Blocks[voxel] = new Voxel(Color.black, false);
                }

                if (delayRecalculation)
                    yield return null;

                if (particle && removalCount > 0)
                {
                    Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
                    GameObject par = Instantiate(particle, point, rot);
                    ParticleSystem.MainModule mod = par.GetComponentInChildren<ParticleSystem>().main;
                    mod.startColor = col;
                    Destroy(par, 1f);
                }
            }
            else if (destructionType == DestructionType.FragmentationRemoval)
            {
                Vector3 localPoint = meshFiler.transform.InverseTransformPoint(point);

                int removalCount = Mathf.FloorToInt(relativeVel * destructionGS.collisionStrength);

                for (int j = 0; j < removalCount; j++)
                {
                    int fragmentSize = relativeSize
                        ? Mathf.RoundToInt(relativeVel * destructionFRS.relativeFragmentScale)
                        : Random.Range(destructionFNRS.minFragmentSize, destructionFNRS.maxFragmentSize);

                    VoxReader.Voxel[] fragmentArray = new VoxReader.Voxel[fragmentSize];

                    int start = GetNearestVoxel(localPoint, overrideMax == -1f ? destructionGS.maxVoxelDistance : overrideMax);

                    if (start == -1)
                        break;

                    for (int k = 0; k < fragmentArray.Length; k++)
                    {
                        if (k == 0)
                        {
                            fragmentArray[k] = new VoxReader.Voxel(To3D(start, length.x, length.y),
                                new Color(data.Blocks[start].color.r * 255, data.Blocks[start].color.g * 255,
                                    data.Blocks[start].color.b * 255, 255));

                            data.Blocks[start] = new Voxel(Color.black, false);
                        }
                        else
                        {
                            int current = GetRandomNeighboar(start);

                            if (current == -1)
                            {
                                VoxReader.Voxel[] temp = fragmentArray;
                                fragmentArray = new VoxReader.Voxel[k];

                                for (int l = 0; l < fragmentArray.Length; l++)
                                {
                                    fragmentArray[l] = temp[l];
                                }

                                break;
                            }
                            else
                            {
                                fragmentArray[k] = new VoxReader.Voxel(To3D(current, length.x, length.y),
                                    new Color(data.Blocks[current].color.r * 255, data.Blocks[current].color.g * 255,
                                        data.Blocks[current].color.b * 255, 255));

                                data.Blocks[current] = new Voxel(Color.black, false);

                                start = current;
                            }
                        }
                    }

                    if (delayRecalculation)
                        yield return null;

                    if (fragmentArray.Length > 0)
                    {
                        GameObject newCube = new GameObject();
                        newCube.transform.position =
                            transform.GetChild(0).TransformPoint(GetMinV3(fragmentArray));

                        newCube.transform.name = transform.name + " Fragment";

                        newCube.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

                        newCube.AddComponent<VoxelStruct>();
                        newCube.GetComponent<VoxelStruct>().BuildObject(fragmentArray, material, allocator, delayRecalculation, isResetable ? this : null);

                        Rigidbody cube = newCube.AddComponent<Rigidbody>();

                        float cubeMass = (totalMass / (length.x * length.y * length.z)) * fragmentArray.Length;

                        cube.mass = cubeMass;
                        cube.drag = drag;
                        cube.angularDrag = angularDrag;
                        cube.interpolation = interpolation;

                        cube.AddForce(normal * destructionGS.collisionForce, ForceMode.Impulse);

                        j += fragmentArray.Length;

                        if (delayRecalculation)
                        {
                            activateOnRecalculation.Add(newCube);
                        }
                    }
                }
            }
            else if (destructionType == DestructionType.PreCalculatedFragments)
            {
                if (destructionPFS.useJobFragmentation)
                {
                    int removalCount = Mathf.FloorToInt((relativeVel * destructionGS.collisionStrength) / ((destructionPFS.fragSphereRadiusMax + destructionPFS.fragSphereRadiusMin) / 2f));

                    if (removalCount > 0)
                    {
                        if (fragmentBuilder != null)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                yield return null;

                                if (fragmentBuilder == null)
                                    break;

                                if (i == 9)
                                    yield break;
                            }
                        }

                        fragmentBuilder = new FragmentBuilder(fragmentsT, removalCount, point);

                        if (delayRecalculation)
                            yield return null;

                        int[] fragments = fragmentBuilder.GetFragments();
                        fragmentBuilder.Dispose();

                        for (int i = 0; i < fragments.Length; i++)
                        {
                            if (fragments[i] == -1 || fragments[i] >= fragmentsT.Length)
                                break;

                            if ((fragmentsT[fragments[i]].position - point).sqrMagnitude > Mathf.Pow(overrideMax == -1f ? destructionGS.maxVoxelDistance : overrideMax, 2))
                                continue;

                            int[] toRemove = fragmentsT[fragments[i]].GetComponent<VoxelFragment>().fragment;

                            for (int j = 0; j < toRemove.Length; j++)
                            {
                                data.Blocks[toRemove[j]].active = false;
                            }
                            //
                            //Destroy(fragmentsT[fragments[i]].gameObject);
                            //
                            fragmentsT[fragments[i]].gameObject.SetActive(true);

                            fragmentsT[fragments[i]].parent = null;
                            Rigidbody fragmentRB = fragmentsT[fragments[i]].GetComponent<Rigidbody>();
                            fragmentRB.AddForce(normal * destructionGS.collisionForce, ForceMode.Impulse);
                            float cubeMass = (totalMass / (length.x * length.y * length.z)) * toRemove.Length;
                            fragmentRB.mass = cubeMass;
                            fragmentRB.drag = drag;
                            fragmentRB.angularDrag = angularDrag;
                            fragmentRB.constraints = constraints;
                            fragmentRB.interpolation = interpolation;
                            fragmentRB.collisionDetectionMode = collisionMode;
                        }

                        fragmentBuilder = null;
                    }
                }
                else
                {
                    int removalCount = Mathf.FloorToInt(relativeVel * destructionGS.collisionStrength);
                    for (int i = 0; i < removalCount;)
                    {
                        float minDistance = -1;
                        int selected = -1;

                        for (int j = 0; j < fragmentsT.Length; j++)
                        {
                            if (!fragmentsT[j].gameObject.activeInHierarchy)
                            {
                                if (minDistance == -1)
                                {
                                    selected = j;
                                    minDistance = (point - fragmentsT[j].position).sqrMagnitude;
                                }
                                else if ((point - fragmentsT[j].position).sqrMagnitude < minDistance)
                                {
                                    selected = j;
                                    minDistance = (point - fragmentsT[j].position).sqrMagnitude;
                                }
                            }
                        }

                        if (selected == -1)
                            break;

                        if ((fragmentsT[selected].position - point).sqrMagnitude > Mathf.Pow(overrideMax == -1f ? destructionGS.maxVoxelDistance : overrideMax, 2))
                            break;

                        int[] toRemove = fragmentsT[selected].GetComponent<VoxelFragment>().fragment;

                        for (int j = 0; j < toRemove.Length; j++)
                        {
                            data.Blocks[toRemove[j]].active = false;
                        }

                        fragmentsT[selected].gameObject.SetActive(true);

                        fragmentsT[selected].parent = null;
                        Rigidbody fragmentRB = fragmentsT[selected].GetComponent<Rigidbody>();
                        fragmentRB.AddForce(normal * destructionGS.collisionForce, ForceMode.Impulse);
                        float cubeMass = (totalMass / (length.x * length.y * length.z)) * toRemove.Length;
                        fragmentRB.mass = cubeMass;
                        fragmentRB.drag = drag;
                        fragmentRB.angularDrag = angularDrag;
                        fragmentRB.constraints = constraints;
                        fragmentRB.interpolation = interpolation;
                        fragmentRB.collisionDetectionMode = collisionMode;

                        i += toRemove.Length;
                    }
                }
            }

            //Collapsing

            if (collapse)
                needsCollapsing = true;

            //Events
            if (!destroyDestructionPercentage && onDestruction != null)
                onDestruction.Invoke(point, relativeVel);

            if (destroyDestructionPercentage)
            {
                destructionPercentage = 1 - (float)GetVoxelCount() / (float)initialVoxelCount;
                if (onDestructionPercentage != null)
                    onDestructionPercentage.Invoke(point, relativeVel, destructionPercentage);
            }

            if (!activeRb && destructionGS.physicsOnCollision)
            {
                activeRb = true;

                meshFiler.GetComponent<MeshCollider>().convex = true;

                Rigidbody myRb = meshFiler.gameObject.AddComponent<Rigidbody>();

                myRb.mass = totalMass;
                myRb.drag = drag;
                myRb.angularDrag = angularDrag;
                myRb.constraints = constraints;
                myRb.interpolation = interpolation;
                myRb.collisionDetectionMode = collisionMode;
            }

            if (destructionCount == 1)
            {
                if (recalculationI != null)
                    yield return recalculationI;

                recalculationI = StartCoroutine(CreateVoxelMesh(meshFiler, false));
            }

            destructionCount--;

        }

        #endregion

        //Contains some logic/index methods
        #region Other

        private void ProcessPivot()
        {
            if (pivotSet)
                return;

#if UNITY_EDITOR
            if (exportMesh)
            {
                string filePath =
                    EditorUtility.SaveFilePanelInProject("Save Procedural Mesh", "Procedural Mesh", "asset", "");
                if (filePath != "")
                {
                    AssetDatabase.CreateAsset(meshFiler.mesh, filePath);
                }
            }
#endif

            pivotSet = true;

            meshFiler.mesh.RecalculateBounds();

            // Settings position offset to avoid wrong placing models in world if there was remvoed empty areas in models.
            meshFiler.transform.localPosition = meshFiler.transform.localPosition + data.PositionOffset;

            if (pivotType == PivotType.Center)
            {
                meshFiler.transform.position = meshFiler.transform.TransformPoint(meshFiler.sharedMesh.bounds.center);
                meshFiler.transform.localPosition = -meshFiler.transform.localPosition;
                meshFiler.transform.localPosition += new Vector3(-0.5f, 0.5f, 0.5f);
            }
            else if (pivotType == PivotType.Min)
            {
                meshFiler.transform.position = meshFiler.transform.TransformPoint(meshFiler.sharedMesh.bounds.min);
                meshFiler.transform.localPosition = -meshFiler.transform.localPosition;
                meshFiler.transform.localPosition += new Vector3(-0.5f, 0.5f, 0.5f);
            }
        }

        //Returns index of the nearest voxel to a point (-1 if not found)
        private int GetNearestVoxel(Vector3 point, float maxDistance)
        {
            int current = 0;
            float minDistance = 999;

            for (int i = 0; i < data.Blocks.Length; i++)
            {
                if (!data.Blocks[i].active)
                    continue;

                float distance = (point - To3D(i, data.Size.x, data.Size.y)).sqrMagnitude;

                if (distance < minDistance)
                {
                    current = i;
                    minDistance = distance;
                }
            }

            if (Mathf.Sqrt(minDistance) > maxDistance || minDistance == 999f)
                return -1;

            return current;
        }

        //Returns a random neighboar to a voxel
        private int GetRandomNeighboar(int voxel)
        {
            int target = Random.Range(0, 6);
            for (int i = 0; i < 6; i++)
            {
                int3 dir = new int3(0, 0, 0);
                dir[target % 3] = 1;
                if (target > 2)
                    dir = -dir;

                Vector3 pos = To3D(voxel, length.x, length.y) + new Vector3(dir.x, dir.y, dir.z);

                if (pos.x >= length.x || pos.x < 0 || pos.y >= length.y || pos.y < 0 || pos.z >= length.z || pos.z < 0)
                    continue;

                int index = To1D(pos);

                if (index >= 0 && index < data.Blocks.Length && data.Blocks[index].active)
                    return index;

                target += 1;
                if (target > 5)
                    target = 0;
            }

            return -1;
        }

        private Vector3 GetMinV3(VoxReader.Voxel[] array)
        {
            Vector3 min = array[0].Position;

            for (int i = 1; i < array.Length; i++)
            {
                if (array[i].Position.x < min.x)
                    min = new Vector3(array[i].Position.x, min.y, min.z);

                if (array[i].Position.y < min.y)
                    min = new Vector3(min.x, array[i].Position.y, min.z);

                if (array[i].Position.z < min.z)
                    min = new Vector3(min.x, min.y, array[i].Position.z);
            }

            return min;
        }

        private Vector3 GetMinV3I(int[] array)
        {
            Vector3 min = To3D(array[0], length.x, length.y);

            for (int i = 1; i < array.Length; i++)
            {
                Vector3 pos = To3D(array[i], length.x, length.y);

                if (pos.x < min.x)
                    min = new Vector3(pos.x, min.y, min.z);

                if (pos.y < min.y)
                    min = new Vector3(min.x, pos.y, min.z);

                if (pos.z < min.z)
                    min = new Vector3(min.x, min.y, pos.z);
            }

            return min;
        }

        //Returns the count of active Voxels
        private int GetVoxelCount()
        {
            int c = 0;

            for (int i = 0; i < data.Blocks.Length; i++)
                if (data.Blocks[i].active)
                    c++;

            return c;
        }

        //Checks if any voxels are left
        private bool Exists()
        {
            for (int i = 0; i < data.Blocks.Length; i++)
                if (data.Blocks[i].active)
                    return true;

            return false;
        }

        //Index Stuff
        private Vector3 To3D(long index, int xMax, int yMax)
        {
            int z = (int)index / (xMax * yMax);
            int idx = (int)index - (z * xMax * yMax);
            int y = idx / xMax;
            int x = idx % xMax;
            return new Vector3(x, y, z);
        }

        private int To1D(Vector3 index)
        {
            return (int)(index.x + length.x * (index.y + length.y * index.z));
        }

        #endregion

        //Loads the fragments for Precalculated Fragments
        #region FragmentCalculation

        private IEnumerator ProcessWebGLFragmentsCoroutine(string path, Action<string> callback, Action notFoundCallback)
        {
            UnityWebRequest wr = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, path));

            yield return wr.SendWebRequest();

            if (wr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error loading web request: " + wr.result + ", " + wr.error, this);
            }

            if (wr.downloadHandler.data == null)
                notFoundCallback.Invoke();
            else
                callback.Invoke(wr.downloadHandler.text);

            FinishModel();

            wr.Dispose();
        }

        //private async UniTask ProcessWebGLFragments(string path, Action<string> callback, Action notFoundCallback)
        //{
        //    Debug.Log($"Processing WebGL fragments: {Path.Combine(ASSET_PATH, path)}");

        //    var voxel = Addressables.LoadAssetAsync<TextAsset>(Path.Combine(ASSET_PATH, path));

        //    await voxel;

        //    //if (voxel is null)
        //    //    notFoundCallback.Invoke();
        //    //else
        //    callback.Invoke(voxel.Result.text);

        //    FinishModel();
        //}

        private void LoadFragments()
        {
            Debug.LogWarning("No Fragment File found, consider precaulculating Fragments in the Editor", this);

            List<int[]> fragments = new List<int[]>();
            List<int> currentFrag = new List<int>();


            VoxelData tempData = new VoxelData(data.Size, new Voxel[data.Blocks.Length], data.PositionOffset);
            //for (int i = 0; i < data.Blocks.Length; i++)
            //{
            //    tempData.Blocks[i] = new Voxel(data.Blocks[i]);
            //}
            Array.Copy(data.Blocks, tempData.Blocks, data.Blocks.Length);

            int so = 0;
            int current = GetNext(tempData);
            while (current != -1 && so < data.Blocks.Length)
            {
                //Frag
                SimpleSphere fragmentSphere = new SimpleSphere(To3D(current, length.x, length.y), Random.Range(
                    destructionPFS.fragSphereRadiusMin,
                    destructionPFS.fragSphereRadiusMax));

                for (int i = 0; i < tempData.Blocks.Length; i++)
                {
                    if (tempData.Blocks[i].active && fragmentSphere.IsInsideSphere(To3D(i, length.x, length.y)))
                    {
                        currentFrag.Add(i);
                        tempData.Blocks[i] = new Voxel(Color.black, false);
                    }
                }

                fragments.Add(currentFrag.ToArray());
                currentFrag.Clear();

                current = GetNext(tempData);
                so++;
            }

            if (so >= data.Blocks.Length)
                Debug.LogError("Calculating fragments caused Stackoverflow, make sure values are correct!", this);
            else
            {
                fragmentParent = new GameObject("FragmentParent").transform;


                fragmentParent.parent = meshFiler.transform;
                fragmentParent.localScale = Vector3.one;
                fragmentParent.localPosition = Vector3.zero;
                fragmentParent.localRotation = Quaternion.identity;
                fragmentsT = new Transform[fragments.Count];

                for (int i = 0; i < fragments.Count; i++)
                {
                    //Gives every Fragment his own color (debugging)
                    if (debug)
                    {
                        Color c = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
                            1);

                        for (int j = 0; j < fragments[i].Length; j++)
                        {
                            data.Blocks[fragments[i][j]].color = c;
                        }
                    }

                    VoxReader.Voxel[] fragmentArray = new VoxReader.Voxel[fragments[i].Length];

                    for (int j = 0; j < fragmentArray.Length; j++)
                    {
                        fragmentArray[j] = new VoxReader.Voxel(To3D(fragments[i][j], length.x, length.y),
                            new Color(data.Blocks[fragments[i][j]].color.r * 255, data.Blocks[fragments[i][j]].color.g * 255, data.Blocks[fragments[i][j]].color.b * 255, 255));
                    }

                    GameObject newCube = new GameObject();
                    newCube.transform.position =
                        transform.GetChild(0).TransformPoint(GetMinV3(fragmentArray));

                    newCube.transform.parent = fragmentParent;
                    newCube.transform.name = transform.name + " Fragment " + i;

                    newCube.transform.localScale = Vector3.one;

                    newCube.AddComponent<VoxelFragment>();
                    newCube.GetComponent<VoxelFragment>().BuildObject(fragmentArray, fragments[i], material, this);

                    fragmentsT[i] = newCube.transform;

                    Rigidbody cube = newCube.AddComponent<Rigidbody>();

                    float cubeMass = (totalMass / (length.x * length.y * length.z)) * fragmentArray.Length;

                    cube.mass = cubeMass;
                    cube.drag = drag;
                    cube.angularDrag = angularDrag;
                    cube.interpolation = interpolation;
                }
            }
        }

        private void ProcessFragments(string frag)
        {
            string[] arrayStrings = frag.Split(';');

            fragmentParent = new GameObject("FragmentParent").transform;

            fragmentParent.tag = "FragmentParent";
            fragmentParent.parent = meshFiler.transform;
            fragmentParent.localScale = Vector3.one;
            fragmentParent.localPosition = Vector3.zero;
            fragmentParent.localRotation = Quaternion.identity;
            fragmentsT = new Transform[arrayStrings.Length];

            for (int i = 0; i < arrayStrings.Length; i++)
            {
                if (arrayStrings[i] == "" || arrayStrings[i] == " ")
                    continue;

                int[] currFrag = Array.ConvertAll(arrayStrings[i].Split(','), s => int.Parse(s));

                if (debug)
                {
                    Color c = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
                        1);

                    for (int j = 0; j < currFrag.Length; j++)
                    {
                        data.Blocks[currFrag[j]].color = c;
                    }
                }

                VoxReader.Voxel[] fragmentArray = new VoxReader.Voxel[currFrag.Length];

                for (int j = 0; j < fragmentArray.Length; j++)
                {
                    fragmentArray[j] = new VoxReader.Voxel(To3D(currFrag[j], length.x, length.y),
                        new Color(data.Blocks[currFrag[j]].color.r * 255, data.Blocks[currFrag[j]].color.g * 255, data.Blocks[currFrag[j]].color.b * 255, 255));
                }

                GameObject newCube = new GameObject();
                newCube.transform.position =
                    transform.GetChild(0).TransformPoint(GetMinV3(fragmentArray));

                newCube.transform.parent = fragmentParent;
                newCube.transform.name = transform.name + " Fragment " + i;

                newCube.transform.localScale = Vector3.one;

                newCube.AddComponent<VoxelFragment>();
                newCube.GetComponent<VoxelFragment>().BuildObject(fragmentArray, currFrag, material, this);

                fragmentsT[i] = newCube.transform;

                Rigidbody cube = newCube.AddComponent<Rigidbody>();

                float cubeMass = (totalMass / (length.x * length.y * length.z)) * fragmentArray.Length;

                cube.mass = cubeMass;
                cube.drag = drag;
                cube.angularDrag = angularDrag;
                cube.interpolation = interpolation;
            }
        }

        public void PrecalculateFragments(int modelIndex)
        {
#if UNITY_WEBGL
            StartCoroutine(ProcessWebGLFragmentsCoroutine(path.Replace(".vox", "_" + modelIndex.ToString() + ".txt"),
                ProcessFragments, LoadFragments));
            //await ProcessWebGLFragments(path.Replace(".vox", "_" + modelIndex.ToString() + ".txt"),
            //    ProcessFragments, LoadFragments);
#else
            if (BetterStreamingAssets.FileExists(path.Replace(".vox", "_" + modelIndex.ToString() + ".txt")))
            {
                ProcessFragments(BetterStreamingAssets.ReadAllText(path.Replace(".vox", "_" + modelIndex.ToString() + ".txt")));
            }
            else
            {
               LoadFragments();
            }
#endif
        }

        /// <summary>
        /// Gets nearest voxel from temp.Blocks and returns it.
        /// </summary>
        private int GetNext(VoxelData temp)
        {
            Vector3Int nearest = new Vector3Int(-1, -1, -1);
            for (int i = 0; i < temp.Blocks.Length; i++)
            {
                if (!temp.Blocks[i].active)
                    continue;

                if (nearest.x == -1)
                {
                    nearest = Vector3Int.RoundToInt(To3D(i, length.x, length.y));
                    continue;
                }

                if (Vector3.Min(nearest, To3D(i, length.x, length.y)) != nearest)
                    nearest = Vector3Int.RoundToInt(To3D(i, length.x, length.y));
            }

            if (nearest.x == -1)
                return -1;

            return To1D(nearest);
        }

        #endregion

        //Some Events
        #region Events

        public void ResetModel(bool removeFragments)
        {
            throw new NotImplementedException("This method was removed because of need to rework after optimizing");

            //if (!isResetable)
            //{
            //    Debug.LogWarning("You can not reset a Voxel Object that is not set to be resetable. Make sure to check the resetable bool!", this);
            //    return;
            //}

            //ForceJobDispose();
            //StopAllCoroutines();
            //destructionCount = 0;

            //destructible = true;
            //destructionPercentage = 0;
            //data = dataCopy;

            //dataCopy = new VoxelData(length, new Voxel[data.Blocks.Length]);

            //for (int i = 0; i < dataCopy.Blocks.Length; i++)
            //{
            //    dataCopy.Blocks[i] = new Voxel(data.Blocks[i]);
            //}

            //if (removeFragments)
            //{
            //    if (destructionType == DestructionType.FragmentationRemoval)
            //    {
            //        VoxelStruct[] frag = FindObjectsOfType<VoxelStruct>();

            //        for (int i = 0; i < frag.Length; i++)
            //        {
            //            if (frag[i].voxOrigin == this)
            //                Destroy(frag[i].transform.root.gameObject);
            //        }
            //    }
            //    else if (destructionType == DestructionType.PreCalculatedFragments)
            //    {
            //        VoxelFragment[] frag = FindObjectsOfType<VoxelFragment>();

            //        for (int i = 0; i < frag.Length; i++)
            //        {
            //            if (frag[i].voxOrigin == this)
            //            {
            //                frag[i].transform.parent = fragmentParent;
            //                frag[i].ResetFrag();
            //            }
            //        }
            //    }
            //}

            //StartCoroutine(CreateVoxelMesh(meshFiler, false));
        }

        private void Reset()
        {
            destructionGS = new DestructionGeneralSettings();
            destructionSS = new DestructionSoundSettings();
            destructionPFS = new DestructionPreFragmentSettings();
            destructionFRS = new DestructionFragmentRSettings();
            destructionFNRS = new DestructionFragmentNRSettings();
        }

        private void OnApplicationQuit()
        {
            ForceJobDispose();
        }

        private void ForceJobDispose()
        {
            if (builder != null)
                builder.Dispose();
            else if (safeBuilder != null)
                safeBuilder.Dispose();
            else if (parallelBuilder != null)
                parallelBuilder.Dispose();

            if (fragmentBuilder != null)
                fragmentBuilder.Dispose();

            if (collapseBuilder != null)
                collapseBuilder.Dispose();
        }

        private void OnDestroy()
        {
            ForceJobDispose();
        }

        #endregion

        //WebGL file loading
        #region WebGLRequests

        public IEnumerator LoadAssetCoroutine(string path, Action<IVoxFile> callback)
        {
            UnityWebRequest wr = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, path));

            yield return wr.SendWebRequest();

            if (wr.result != UnityWebRequest.Result.Success)
                Debug.LogError("Error loading web request: " + wr.result + ", " + wr.error, this);

            if (wr.downloadHandler.data == null)
                Debug.LogError("Vox file not found!", this);

            IVoxFile file = VoxReader.VoxReader.Read(wr.downloadHandler.data);

            callback.Invoke(file);

            wr.Dispose();
        }

        //public async UniTask LoadAssetAsync(string path, Action<IVoxFile> callback)
        //{
        //    Debug.Log($"Trying to load asset from: {Path.Combine(ASSET_PATH, path)}");

        //    var voxel = Addressables.LoadAssetAsync<VoxelAsset>(Path.Combine("Assets/RemoteModels/", path));

        //    await voxel;

        //    IVoxFile file = VoxReader.VoxReader.Read(voxel.Result.Bytes);

        //    callback.Invoke(file);
        //}

        #endregion
    }

    [Serializable]
    public class DestructionGeneralSettings
    {
        [Tooltip("The mimimun relative velocity magnitude between the colliding objects to start destruction")]
        public float minCollisionMag = 0.35f;

        [Tooltip("The amount of destruction")]
        public float collisionStrength = 30;

        [Tooltip("The Force applied to the Fragment rigidbody on destruction")]
        public float collisionForce = 0.1f;

        [Tooltip("Makes the model a physic object on collision")]
        public bool physicsOnCollision = false;

        [Tooltip("The maximum distance between collision point and the Voxel/Fragment")]
        public float maxVoxelDistance = 10;
    }

    [Serializable]
    public class DestructionSoundSettings
    {
        [Tooltip("The collision clips for collisions, a random one gets selected, make sure it is setup correctly")]
        public string[] collisionClip = new string[] { "Destruction1" };

        [Tooltip("The volume scale")]
        public float soundVolumeScale = 100;
    }

    [Serializable]
    public class DestructionFragmentNRSettings
    {
        [Tooltip("Minimum fragment size in Voxels")]
        public int minFragmentSize = 2;

        [Tooltip("Maximum fragment size in Voxel")]
        public int maxFragmentSize = 10;
    }

    [Serializable]
    public class DestructionFragmentRSettings
    {
        [Tooltip("The relative fragment scale")]
        public float relativeFragmentScale = 0.5f;
    }

    [Serializable]
    public class DestructionPreFragmentSettings
    {
        [Tooltip("Defines if a Job should be used to calculate the fragments to remove")]
        public bool useJobFragmentation = true;

        [Space]

        [Tooltip("The minimum sphere radius used for fragment calculation")]
        public float fragSphereRadiusMin = 2;
        [Tooltip("The maxmimum sphere radius used for fragment calculation")]
        public float fragSphereRadiusMax = 5;
    }
}
