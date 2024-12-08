using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Economic;
using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(PartsCollector))]
    public class CarSetup : MonoBehaviour, IPaintable
    {
        [SerializeField] private PartsCollector _collector;
        [SerializeField] private CarConfiguration _configuration;

        private List<PaintWindow> _paintWindows;
        private List<PaintPart> _paintParts;
        private List<Wheel> _startWheels;
        private List<Wheel> _currentWheels;
        private bool _didPainted;
        private GameObject _gameObject;

        [field: SerializeField] public WheelSize WheelSize { get; private set; }

        public float MinDestroyedPercent => _configuration.MinDestroyedPercent;

        public CarSetup Initialize()
        {
            _gameObject = gameObject;
            _gameObject.SetActive(false);
            return this;
        }

        public List<AudioSource> Initialize(
            EconomicService economicService,
            ObjectPool<SpawnableSound> pool)
        {
            _gameObject.SetActive(true);
            Dictionary<CarPart, PartSettings> configuration = _configuration.GetConfiguration();

            if (_didPainted == true)
            {
                foreach (Wheel wheel in _startWheels)
                    _collector.Parts.Remove(wheel);

                foreach (Wheel wheel in _currentWheels)
                    _collector.Parts.Add(wheel);
            }

            foreach (VitalityPart part in _collector.Parts)
            {
                part.Initialize(
                    configuration[part.Name],
                    pool,
                    economicService.SpawnTwoCoin,
                    economicService.SpawnCoin);

                if (part.Name is CarPart.Window or CarPart.Headlight or CarPart.Wheel == false)
                    part.SetShader();
            }

            return _collector.Sounds;
        }

        public void InitializePaint()
        {
            _gameObject.SetActive(true);
            _didPainted = true;
            _collector.Paints.ForEach(item => item.Initialize());

            _startWheels = _collector.Parts
                .Where(item => item is Wheel == true)
                .Cast<Wheel>().ToList();
            _currentWheels = _startWheels.ToList();

            _paintWindows = _collector.Paints
                .Where(item => item is PaintWindow == true)
                .Cast<PaintWindow>().ToList();

            _paintParts = _collector.Paints
                .Where(item => item is PaintWindow == false).ToList();
        }

        public PartHighlighter GetHighlighter(Material highlighted)
        {
            List<IHighlightable> glasses =
                _collector.Parts.Where(item => item is Glass == true).Cast<IHighlightable>().ToList();
            List<IHighlightable> wheels =
                _collector.Parts.Where(item => item is Wheel == true).Cast<IHighlightable>().ToList();
            wheels = wheels.Where(item => item.Renderer != null).ToList();

            return new PartHighlighter(glasses, wheels, highlighted);
        }

        public Texture2D GetTexture()
        {
            _collector.CameraGameObject.SetActive(true);
            _gameObject.SetActive(true);
            RenderTexture texture = _collector.Camera.targetTexture;
            int width = texture.width;
            int height = texture.height;

            Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, texture.mipmapCount, false);
            _collector.Camera.Render();
            Graphics.CopyTexture(texture, result);
            _gameObject.SetActive(false);
            _collector.CameraGameObject.SetActive(false);
            texture.Release();
            return result;
        }

        public IReadOnlyList<VitalityPart> GetParts()
        {
            return _collector.Parts;
        }

        public void ChangeWheels(Wheel template, float size)
        {
            List<Wheel> toAdd = new List<Wheel>();

            foreach (Wheel wheel in _currentWheels)
            {
                Transform parent = wheel.transform.parent;
                Wheel newWheel = Instantiate(template, parent);
                newWheel.transform.localScale = new Vector3(size, size, size);
                toAdd.Add(newWheel);

                if (_startWheels.Contains(wheel) == true)
                    wheel.gameObject.SetActive(false);
                else
                    Destroy(wheel.gameObject);
            }

            _currentWheels.Clear();
            _currentWheels.AddRange(toAdd);
        }

        public void PaintWindows(Material material)
        {
            foreach (PaintWindow window in _paintWindows)
                window.Paint(material);
        }

        public void PaintCar(Material material)
        {
            foreach (PaintPart part in _paintParts)
                part.Paint(material);
        }

        public void ReturnWheels()
        {
            bool isNeedActivation = false;

            foreach (Wheel wheel in _currentWheels)
            {
                if (_startWheels.Contains(wheel) == true)
                    wheel.gameObject.SetActive(true);
                else
                {
                    Destroy(wheel.gameObject);
                    isNeedActivation = true;
                }
            }

            if (isNeedActivation == true)
            {
                foreach (Wheel wheel in _startWheels)
                    wheel.gameObject.SetActive(true);
            }

            _currentWheels.Clear();
            _currentWheels.AddRange(_startWheels);
        }

        public void ReturnWindows()
        {
            foreach (PaintWindow window in _paintWindows)
                window.Return();
        }

        public void ReturnPaint()
        {
            foreach (PaintPart part in _paintParts)
                part.Return();
        }
    }
}