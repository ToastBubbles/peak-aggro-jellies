using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AggressiveJellies
{
    public class JellyfishDebugText : MonoBehaviour
    {
        public TextMesh textMesh;
        private Transform cam;

        public void Init(Transform cam)
        {
            this.cam = cam;
        }

        public void SetText(string text)
        {
            if (textMesh != null)
            {
                textMesh.text = text;
            }
        }

        void LateUpdate()
        {
            if (textMesh != null && cam != null)
            {
                textMesh.transform.rotation = Quaternion.LookRotation(textMesh.transform.position - cam.position);
            }
        }
    }

}
