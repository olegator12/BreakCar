using System;
using System.Collections.Generic;
using UnityEngine;

namespace Destruction
{
    [RequireComponent(typeof(MeshFilter))]
    public abstract class DeformableMesh : VitalityPart
    {
        [SerializeField] private AudioSource _sound;

        private float _deformationRadius = 0.1f;
        private float _deformationForce = 0.5f;

        private MeshFilter _filter;
        private MeshCollider _collider;
        private Mesh _mesh;

        public override void Initialize(
            PartSettings settings,
            ObjectPool<SpawnableSound> pool,
            Action<Vector3, Vector3> onBroke,
            Action<Vector3, Vector3> onFirstTimeHitTook)
        {
            base.Initialize(settings, pool, onBroke, onFirstTimeHitTook);
            _filter = GetComponent<MeshFilter>();
            _mesh = _filter.mesh;
            _deformationRadius = settings.DeformationRadius;
            _deformationForce = settings.DeformationForce;

            if (TryGetComponent(out MeshCollider meshCollider) == false)
                return;

            _collider = meshCollider;
        }

        public override void Accept(IPartVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Break(
            Vector3 contactPosition,
            Vector3 normal,
            Vector3 relativeVelocity,
            float deformationModifier,
            IReadOnlyList<DamageType> damageTypes,
            float damageValue)
        {
            bool didDeform = false;
            Vector3[] vertices = _mesh.vertices;
            Vector3 point = Transform.InverseTransformPoint(contactPosition);
            Vector3 velocity = Transform.InverseTransformVector(relativeVelocity);

            PlaySound();
            SetLastContact(contactPosition, normal);
            TakeDamage(damageTypes, damageValue);
            OnContact();

            for (int i = 0; i < _mesh.vertexCount; i++)
            {
                float distance = Vector3.Distance(point, vertices[i]);

                if (distance > _deformationRadius)
                    continue;

                Vector3 deformation = velocity * ((_deformationRadius - distance) * _deformationForce * deformationModifier);
                vertices[i] += deformation;
                didDeform = true;
            }

            if (didDeform == false)
                return;

            _mesh.vertices = vertices;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();

            if (_collider != null)
                _collider.sharedMesh = _mesh;
        }

        public void RegisterSound(AudioSource sound)
        {
            _sound = sound;
        }

        public void PlaySound()
        {
            PlaySound(_sound.clip);
        }

        public abstract void OnContact();
    }
}