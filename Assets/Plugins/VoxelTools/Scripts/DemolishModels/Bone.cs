using System.Collections.Generic;
using UnityEngine;

namespace ClassesElements
{
    [SerializeField]
    public class Bone : ScriptableObject
    {
        [field: SerializeField] private List<GameObject> bones;
        public void AddToList(GameObject item)
        {
            bones.Add(item);
        }
        public void RemoveToList(GameObject item) 
        {
            bones.Remove(item);
        }
        public void SetList(List<GameObject> list)
        {
            bones = list;
        } 
        public List<GameObject> GetList()
        {
            return bones;
        }
        public void GetListInDebug()
        {
            foreach (var i in bones)
            {
                Debug.Log(i.ToString());
            }
        }
    }
}