using System.Collections.Generic;
using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Collider))]
    public abstract class Glass : VitalityPart, IHighlightable
    {
        [SerializeField] private GameObject _partsHolder;
        [SerializeField] private List<GlassPart> _parts;
        [SerializeField] private AudioSource _breaking;
        [SerializeField] private AudioSource _hitting;

        private Transform _holderTransform;
        private Collider _collider;
        private PartSettings _settings;
        private int _skipPassedCount;
        private bool _didCrack;

        public GameObject PartsHolder => _partsHolder;

        public MeshRenderer Renderer { get; private set; }

        public override void OnInitialize()
        {
            _parts ??= new List<GlassPart>();
            Renderer = GetComponent<MeshRenderer>();
            _collider = GetComponent<Collider>();
            Transform holderTransform = _partsHolder.transform;

            for (int i = 0; i < holderTransform.childCount; i++)
                _parts.Add(holderTransform.GetChild(i).GetComponent<GlassPart>());
        }

        public override void Accept(IPartVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void OnBroke()
        {
            PlaySound(_breaking.clip);
            Crack();
            _collider.enabled = false;

            foreach (GlassPart part in _parts)
                part.Burst(LastNormal);
        }

        public override void OnFirstTimeHitTook()
        {
        }

        public override void Break(
            Vector3 contactPosition,
            Vector3 normal,
            Vector3 relativeVelocity,
            float deformationModifier,
            IReadOnlyList<DamageType> damageTypes,
            float damageValue)
        {
            if (_skipPassedCount >= Settings.SkipCount)
                Crack();
            else
                _skipPassedCount++;

            PlaySound(_hitting.clip);
            SetLastContact(contactPosition, normal);
            TakeDamage(damageTypes, damageValue);
        }

        public void RegisterHolder(GameObject holder)
        {
            _partsHolder = holder;
        }

        public void RegisterSound(AudioSource breaking, AudioSource hitting)
        {
            _breaking = breaking;
            _hitting = hitting;
        }

        public override bool TryStick(Transform _)
        {
            return false;
        }

        private void Crack()
        {
            if (_didCrack == true)
                return;

            _didCrack = true;
            _partsHolder.SetActive(true);
            Renderer.enabled = false;
        }
    }
}