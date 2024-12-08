using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(ObjectPopUpper))]
    public class PopUpEventButton : EventButton
    {
        private ObjectPopUpper _popUpper;

        public override void OnInitialize()
        {
            _popUpper = GetComponent<ObjectPopUpper>();
        }

        public void PopUp() => _popUpper.PopUp();

        public void Stop() => _popUpper.Stop();
    }
}