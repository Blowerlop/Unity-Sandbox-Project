using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Girod_Nathan
{
    namespace Utilities
    {
        public class Utilities : MonoBehaviour
        {
            public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
            {
                [SerializeField, HideInInspector]
                private List<TKey> keyData = new List<TKey>();
	
                [SerializeField, HideInInspector]
                private List<TValue> valueData = new List<TValue>();

                void ISerializationCallbackReceiver.OnAfterDeserialize()
                {
                    this.Clear();
                    for (int i = 0; i < this.keyData.Count && i < this.valueData.Count; i++)
                    {
                        this[this.keyData[i]] = this.valueData[i];
                    }
                }

                void ISerializationCallbackReceiver.OnBeforeSerialize()
                {
                    this.keyData.Clear();
                    this.valueData.Clear();

                    foreach (var item in this)
                    {
                        this.keyData.Add(item.Key);
                        this.valueData.Add(item.Value);
                    }
                }
            }
            public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3),
                int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, Vector3 rotation = default(Vector3), 
                TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
            {
                if (color == null) color = Color.white;
                return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, rotation, textAlignment,
                    sortingOrder);
            }
    
    
            public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color,
                TextAnchor textAnchor, Vector3 rotation, TextAlignment textAlignment, int sortingOrder)
            {
                GameObject go = new GameObject("World Text", typeof(TextMesh));
                Transform transfo = go.transform;
                transfo.SetParent(parent, false);
                transfo.localPosition = localPosition;
                transfo.rotation = Quaternion.Euler(rotation);
                TextMesh textMesh = go.GetComponent<TextMesh>();
                textMesh.anchor = textAnchor;
                textMesh.alignment = textAlignment;
                textMesh.text = text;
                textMesh.fontSize = fontSize;
                textMesh.color = color;
                textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
                return textMesh;
            }
        } 
    }
    
}
