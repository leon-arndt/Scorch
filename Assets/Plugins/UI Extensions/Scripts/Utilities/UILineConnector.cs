/// Credit Alastair Aitchison
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/123/uilinerenderer-issues-with-specifying
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/UI Line Connector")]
    [RequireComponent(typeof(UILineRenderer))]
    [ExecuteInEditMode]
    public class UILineConnector : MonoBehaviour
    {

        // The elements between which line segments should be drawn
        //public RectTransform[] transforms;
        public List<RectTransform> transforms;
        private Vector2[] previousPositions;
        private RectTransform canvas;
        private RectTransform rt;
        private UILineRenderer lr;

        private void Awake()
        {
            canvas = GetComponentInParent<RectTransform>().GetParentCanvas().GetComponent<RectTransform>();
            rt = GetComponent<RectTransform>();
            lr = GetComponent<UILineRenderer>();

            transforms = new List<RectTransform>();
            //Execute(); //remove this
        }

        // Update is called once per frame
        void Execute()
        {
            if (transforms == null || transforms.Count < 1)
            {
                return;
            }
            //Performance check to only redraw when the child transforms move
            if (previousPositions != null && previousPositions.Length == transforms.Count)
            {
                bool updateLine = false;
                for (int i = 0; i < transforms.Count; i++)
                {
                    if (!updateLine && previousPositions[i] != transforms[i].anchoredPosition)
                    {
                        updateLine = true;
                    }
                }
                if (!updateLine) return;
            }

            // Get the pivot points
            Vector2 thisPivot = rt.pivot;
            Vector2 canvasPivot = canvas.pivot;

            // Set up some arrays of coordinates in various reference systems
            Vector3[] worldSpaces = new Vector3[transforms.Count];
            Vector3[] canvasSpaces = new Vector3[transforms.Count];
            Vector2[] points = new Vector2[transforms.Count];

            // First, convert the pivot to worldspace
            for (int i = 0; i < transforms.Count; i++)
            {
                worldSpaces[i] = transforms[i].TransformPoint(thisPivot); //throws an error
            }

            // Then, convert to canvas space
            for (int i = 0; i < transforms.Count; i++)
            {
                canvasSpaces[i] = canvas.InverseTransformPoint(worldSpaces[i]);
            }

            // Calculate delta from the canvas pivot point
            for (int i = 0; i < transforms.Count; i++)
            {
                points[i] = new Vector2(canvasSpaces[i].x, canvasSpaces[i].y);
            }

            /*for (int i = 0; i < transforms.Length; i++) {
                points[i] = new Vector2(transforms[i].position.x, transforms[i].position.y); //throws an error
            }*/


            // And assign the converted points to the line renderer
            lr.Points = points;
            lr.RelativeSize = false;
            lr.drivenExternally = true;

            previousPositions = new Vector2[transforms.Count];
            for (int i = 0; i < transforms.Count; i++)
            {
                previousPositions[i] = transforms[i].anchoredPosition;
            }
        }

        public void SetTransforms(List<RectTransform> rectTransformsList) {
            transforms.Clear();
            for (int i = 0; i < rectTransformsList.Count; i++) { //hardcoded size, was 2
                transforms.Add(rectTransformsList[i]);
            }

            Execute();
        }
    }
}