using UnityEngine;
using Zenject;
using LastConvoy.Configs;

namespace LastConvoy.Views.Helicopter
{
    public class HelicopterBladesView : MonoBehaviour
    {
        private HelicopterConfig _config;

        [Inject]
        public void Construct(HelicopterConfig config)
        {
            _config = config;
        }

        private void Update()
        {
            transform.Rotate(0f, _config.BladeRotationSpeed * Time.deltaTime, 0f);
        }
    }
}
