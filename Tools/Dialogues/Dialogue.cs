using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    [System.Serializable]
    public class Dialogue
    {
        public string name;
        //[TextArea(3,10)]
        public List<string> sentences = new List<string>();

        public class DialogueLine
        {

            public string actorsName;
            public string line;

        }
    }
}
