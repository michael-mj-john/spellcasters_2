using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Edwon.VR.Gesture
{

    public class GestureTrail : MonoBehaviour
    {
        CaptureHand registeredHand;
        int lengthOfLineRenderer = 50;
        List<Vector3> displayLine;
        LineRenderer currentRenderer;
        Color defaultStartColor, defaultEndColor;
        Color initialColor, initialTransColor, finalColor, finalTransColor;
        Material material;
        public bool listening = false;
        bool currentlyInUse = false;
        float transitionTimer = 0f; //Used to fade out the latest trail.
        // Use this for initialization
        void Start()
        {
            //Added to pass allow for passing of colors and material.
            //Set initial color and it's transparent version to be used in the fade out.
            if (initialColor == null) initialColor = Color.red;
            initialTransColor = initialColor;
            initialTransColor.a = 0;

            //Set final color and it's transparent version to be used in the fade out.
            if (finalColor == null) finalColor = Color.blue;
            finalTransColor = finalColor;
            finalTransColor.a = 0;

            //Find default material.
            if (material == null) material = new Material(Shader.Find("Particles/Additive"));

            //Flag as currently in use.
            currentlyInUse = true;
            displayLine = new List<Vector3>();
            currentRenderer = CreateLineRenderer(initialColor, finalColor, material);
        }
        private void Update()
        {
            //If stopped listening and transition timer is less than one, lerp the trail's colors to transparent.
            if(!listening && transitionTimer <= 1)
            {
                transitionTimer += Time.deltaTime;
                currentRenderer.startColor = Color.Lerp(initialColor, initialTransColor, transitionTimer);
                currentRenderer.endColor = Color.Lerp(finalColor, finalTransColor, transitionTimer);
            }
            else if(!listening && transitionTimer > 1)
            {
                currentRenderer.startColor = initialTransColor;
                currentRenderer.endColor = finalTransColor;
            }

        }
        public void SetDefaultColor(Color start, Color end, Material material0 = null)
        {
            defaultStartColor = start;
            defaultEndColor = end;
            material = material0;

            UpdateRenderer(defaultStartColor, defaultEndColor, material);
        }
        public void ResetRenderer()
        {
            UpdateRenderer(defaultStartColor, defaultEndColor, material);
        }    
        public void UpdateRenderer(Color color1, Color color2, Material material0 = null)
        {
            initialColor = color1;
            initialTransColor = initialColor;
            initialTransColor.a = 0;

            finalColor = color2;
            finalTransColor = finalColor;
            finalTransColor.a = 0;

            material = material0;

            if (currentRenderer != null)
            {
                currentRenderer.startColor = initialColor;
                currentRenderer.endColor = finalColor;
                currentRenderer.material = material;
            }
        }

        void OnEnable()
        {
            if (registeredHand != null)
            {
                SubscribeToEvents();
            }
        }

        void SubscribeToEvents()
        {
            registeredHand.StartCaptureEvent += StartTrail;
            registeredHand.ContinueCaptureEvent += CapturePoint;
            registeredHand.StopCaptureEvent += StopTrail;
        }

        void OnDisable()
        {
            if (registeredHand != null)
            {
                UnsubscribeFromEvents();
            }
        }

        void UnsubscribeFromEvents()
        {
            registeredHand.StartCaptureEvent -= StartTrail;
            registeredHand.ContinueCaptureEvent -= CapturePoint;
            registeredHand.StopCaptureEvent -= StopTrail;
        }

        void UnsubscribeAll()
        {

        }

        void OnDestroy()
        {
            currentlyInUse = false;
        }

        LineRenderer CreateLineRenderer(Color c1, Color c2, Material material = null)
        {
            GameObject myGo = new GameObject("Trail Renderer");
            myGo.transform.parent = transform;
            myGo.transform.localPosition = Vector3.zero;

            LineRenderer lineRenderer = myGo.AddComponent<LineRenderer>();
            lineRenderer.material = material;
            lineRenderer.SetColors(c1, c2);
            lineRenderer.SetWidth(0.01F, 0.05F);
            lineRenderer.SetVertexCount(0);
            return lineRenderer;
        }

        public void RenderTrail(LineRenderer lineRenderer, List<Vector3> capturedLine)
        {
            if (capturedLine.Count == lengthOfLineRenderer)
            {
                lineRenderer.SetVertexCount(lengthOfLineRenderer);
                lineRenderer.SetPositions(capturedLine.ToArray());
            }
        }

        public void StartTrail()
        {
            if (currentRenderer != null)
                currentRenderer.SetColors(initialColor, finalColor);
            else
                currentRenderer = CreateLineRenderer(initialColor, finalColor);
            displayLine.Clear();
            listening = true;
        }

        public void CapturePoint(Vector3 rightHandPoint)
        {
            displayLine.Add(rightHandPoint);
            currentRenderer.SetVertexCount(displayLine.Count);
            currentRenderer.SetPositions(displayLine.ToArray());
        }

        public void CapturePoint(Vector3 myVector, List<Vector3> capturedLine, int maxLineLength)
        {
            if (capturedLine.Count >= maxLineLength)
            {
                capturedLine.RemoveAt(0);
            }
            capturedLine.Add(myVector);
        }

        public void StopTrail()
        {
            Color start = currentRenderer.startColor;
            Color end = currentRenderer.endColor;
            start.a = 0.1f;
            end.a = 0.1f;
            listening = false;
            //Resets the timer so fade out is possible.
            transitionTimer = 0;
        }

        public void ClearTrail()
        {
            currentRenderer.SetVertexCount(0);
        }

        public bool UseCheck()
        {
            return currentlyInUse;
        }

        public void AssignHand(CaptureHand captureHand)
        {
            currentlyInUse = true;
            registeredHand = captureHand;
            SubscribeToEvents();

        }

    }
}
