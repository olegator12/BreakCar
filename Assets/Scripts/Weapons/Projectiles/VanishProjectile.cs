using System;
using System.Collections.Generic;
using Destruction;
using UnityEngine;

namespace Weapons
{
    public class VanishingProjectile : StandardProjectile
    {
        [SerializeField] private float _holeRadius;

        private Func<Vector3, Vector3, Transform> _holeCreationHandler;

        public void Initialize(Func<Vector3, Vector3, Transform> holeCreationHandler)
        {
            _holeCreationHandler = holeCreationHandler;
        }

        public override void OnStickPrepare(IBreakable breakable, Vector3 position, Collision collision)
        {
            if (breakable is Glass == true)
                return;

            //position -= normal * _holeRadius;
            ContactPoint contact = collision.GetContact(0);
            breakable.TryStick(_holeCreationHandler.Invoke(contact.point, contact.normal));
        }

        public override void OnContactComplete()
        {
            Push();
        }
    }
}