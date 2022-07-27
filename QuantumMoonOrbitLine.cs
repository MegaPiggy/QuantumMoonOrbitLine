using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuantumMoonOrbitLine
{
    public class QuantumMoonOrbitLine : OrbitLine
    {
        private Material _lineMaterial = null;

        private QuantumMoon _quantumMoon;

        private const int _defaultNumVerts = 256;

        private const int _defaultWidth = 50;

        private Color _defaultColor = Color.white;

        public override void Awake()
        {
            if (_lineMaterial == null) _lineMaterial = Main.FindResourceOfTypeAndName<Material>("Effects_SPA_OrbitLine_mat");

            _quantumMoon = Locator.GetQuantumMoon();
            _astroObject = Locator.GetAstroObject(AstroObject.Name.QuantumMoon);

            _fade = true;

            _color = new Color32(184, 191, 200, byte.MaxValue);

            _lineRenderer = gameObject.GetAddComponent<LineRenderer>();
            _lineRenderer.startWidth = _defaultWidth;
            _lineRenderer.endWidth = _defaultWidth;
            _lineRenderer.material = new Material(_lineMaterial);
            _lineRenderer.textureMode = LineTextureMode.Stretch;
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.loop = false;

            _numVerts = _defaultNumVerts;

            base.Awake();

            _lineWidth = 0.2f;

            GlobalMessenger<OWRigidbody>.AddListener("QuantumMoonChangeState", OnQuantumMoonChangeState);

            Main.FireOnNextUpdate(InitializeLineRenderer);
        }

        public void OnQuantumMoonChangeState(OWRigidbody body)
        {
            if (body == _quantumMoon._moonBody)
            {
                Update();
            }
        }

        private OWRigidbody GetPrimaryBodyFromStateIndex()
        {
            int stateIndex = _quantumMoon._stateIndex;

            int elementIndex = -1;

            for (int currentIndex = 0; currentIndex < _quantumMoon._orbits.Length; currentIndex++)
            {
                if (_quantumMoon._orbits[currentIndex].GetStateIndex() == stateIndex)
                {
                    elementIndex = currentIndex;
                    break;
                }
            }

            return elementIndex != -1 ? _quantumMoon._orbits[elementIndex].GetAttachedOWRigidbody() : Locator.GetAstroObject(AstroObject.Name.Sun).GetOWRigidbody();
        }

        public override void Update()
        {
            OWRigidbody primaryBody = GetPrimaryBodyFromStateIndex();
            if (primaryBody != null)
            {
                Vector3 vector3 = _astroObject.transform.position - primaryBody.transform.position;
                Vector3 normalized = Vector3.Cross(primaryBody.GetRelativeVelocity(_astroObject.GetAttachedOWRigidbody()), vector3).normalized;
                float magnitude = vector3.magnitude;
                transform.position = primaryBody.transform.position;
                transform.rotation = Quaternion.LookRotation(vector3, normalized);
                transform.localScale = Vector3.one * magnitude;
                float orbitLine = this.DistanceToOrbitLine(primaryBody.transform.position, normalized, magnitude, Locator.GetActiveCamera().transform.position);
                float fade = CalcFade(orbitLine);
                _lineRenderer.widthMultiplier = Mathf.Min(orbitLine * (_lineWidth / 1000f), _maxLineWidth);
                _lineRenderer.startColor = new Color(_color.r, _color.g, _color.b, _color.a * fade * fade);
            }
        }
    }
}
