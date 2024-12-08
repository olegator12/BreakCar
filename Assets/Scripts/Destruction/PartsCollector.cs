using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Destruction
{
    [Serializable]
    public class PartsCollector : MonoBehaviour
#if UNITY_EDITOR
        , IPartVisitor
#endif
    {
        private const string SaveLocation = "Assets/Mesh/Prefractured/";
        private const string MaterialPath = "InsideGlass";

        [SerializeField] private int _glassFragmentsCount = 5;
        [SerializeField] private List<GameObject> _holders;
        [SerializeField] private bool _didGenerate;
        [SerializeField] private SoundProvider _soundProvider;

        [field: SerializeField] public List<Glass> Glasses { get; private set; }

        [field: SerializeField] public List<VitalityPart> Parts { get; private set; }

        [field: SerializeField] public List<PaintPart> Paints { get; private set; }

        [field: SerializeField] public Camera Camera { get; private set; }

        [field: SerializeField] public GameObject CameraGameObject { get; private set; }

        public List<AudioSource> Sounds => new ()
            { _soundProvider.Hitting, _soundProvider.GlassBreaking, _soundProvider.GlassHitting };

#if UNITY_EDITOR
        public void Generate()
        {
            if (_didGenerate == true)
                return;

            AssetDatabase.CreateFolder(SaveLocation, name);
            CollectGlasses();
            CollectHolders();
            EditorUtility.SetDirty(gameObject);
            AssetDatabase.SaveAssetIfDirty(gameObject);
            AssetDatabase.Refresh();
        }

        public void Clear()
        {
            _didGenerate = false;
            Glasses.Clear();
            Parts.Clear();
            Paints.Clear();

            foreach (GameObject holder in _holders)
                DestroyImmediate(holder);

            _holders.Clear();
            EditorUtility.SetDirty(gameObject);
            AssetDatabase.SaveAssetIfDirty(gameObject);
            AssetDatabase.Refresh();
        }

        public void Visit(DeformableMesh deformableMesh)
        {
            Parts.Add(deformableMesh);
            deformableMesh.RegisterSound(_soundProvider.Hitting);
        }

        public void Visit(Glass glass)
        {
            Glasses.Add(glass);
            Parts.Add(glass);
            glass.RegisterSound(_soundProvider.GlassBreaking, _soundProvider.GlassHitting);
        }

        public void Visit(Wheel wheel)
        {
            Parts.Add(wheel);
        }

        public void Visit(PaintPart paint)
        {
            Paints.Add(paint);
        }

        private void CollectGlasses()
        {
            Transform origin = transform;
            Queue<Transform> iterations = new ();

            CollectParts(origin, iterations);

            while (iterations.Count > 0)
            {
                Transform parent = iterations.Dequeue();

                if (Camera == false)
                    FindCamera(parent);

                if (parent.TryGetComponent(out VitalityPart part) == true)
                    part.Accept(this);

                if (parent.TryGetComponent(out PaintPart paint) == true)
                    paint.Accept(this);

                if (parent.childCount <= 0)
                    continue;

                CollectParts(parent, iterations);
            }
        }

        private void CollectHolders()
        {
            List<Prefracture> fractures = new ();

            foreach (var glassObject in Glasses.Select(glass => glass.gameObject))
            {
                if (glassObject.TryGetComponent(out Prefracture fracture) == false)
                    fracture = glassObject.AddComponent<Prefracture>();

                fractures.Add(fracture);
            }

            for (int i = 0; i < Glasses.Count; i++)
            {
                GameObject glassObject = Glasses[i].gameObject;
                Material material = Resources.Load<Material>(MaterialPath);

                if (material == null)
                    throw new NullReferenceException();

                fractures[i].fractureOptions = new FractureOptions
                {
                    fragmentCount = _glassFragmentsCount,
                    insideMaterial = material,
                };
                fractures[i].prefractureOptions = new PrefractureOptions
                {
                    saveLocation = $"{SaveLocation}{name}",
                    saveFragmentsToDisk = true,
                };
                fractures[i].callbackOptions = new CallbackOptions();
                fractures[i].triggerOptions = new TriggerOptions();

                GameObject holder = fractures[i].ComputeFracture();
                Transform holderTransform = holder.transform;
                glassObject.SetActive(true);

                DestroyImmediate(fractures[i]);
                DestroyImmediate(glassObject.GetComponent<Rigidbody>());

                Glasses[i].RegisterHolder(holder);
                CollectGlassParts(holderTransform, Glasses[i]);

                if (Glasses[i] is Window == true)
                {
                    if (Glasses[i].TryGetComponent(out PaintWindow window) == true)
                        window.RegisterHolder(holderTransform);
                }

                holder.SetActive(false);
                _holders.Add(holder);
            }

            _didGenerate = true;
        }

        private void CollectParts(Transform transform, Queue<Transform> queue)
        {
            for (int i = 0; i < transform.childCount; i++)
                queue.Enqueue(transform.GetChild(i));
        }

        private void CollectGlassParts(Transform holder, Glass glass)
        {
            for (int i = 0; i < holder.childCount; i++)
            {
                Transform child = holder.GetChild(i);
                DestroyImmediate(child.GetComponent<Rigidbody>());
                child.gameObject.AddComponent<GlassPart>().Initialize();
            }
        }

        private void FindCamera(Transform holder)
        {
            if (holder.TryGetComponent(out Camera result) == false)
                return;

            Camera = result;
            CameraGameObject = result.gameObject;
            CameraGameObject.SetActive(false);
        }
#endif
    }
}