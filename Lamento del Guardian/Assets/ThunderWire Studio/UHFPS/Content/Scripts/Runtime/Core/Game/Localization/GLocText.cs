using UnityEngine;
using UnityEngine.Events;

namespace UHFPS.Runtime
{
    public class GLocText : MonoBehaviour
    {
        public GString GlocKey;
        public bool ObserveMany;
        public UnityEvent<string> OnUpdateText;

        private bool _isInitialized;
        
        private void OnEnable()
        {
            if (!GameLocalization.HasReference || _isInitialized)
                return;

            if (!ObserveMany) GlocKey.SubscribeGloc(text => OnUpdateText?.Invoke(text));
            else GlocKey.SubscribeGlocMany(text => OnUpdateText?.Invoke(text));
            _isInitialized = true;
        }
    }
}