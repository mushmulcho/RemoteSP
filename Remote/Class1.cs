using System;

namespace Remote
{

    public class cTransfer : MarshalByRefObject
    {
        [Serializable]
        public struct kAction
        {
            public string s_Command;
            public string s_Computer;
        };

        [Serializable]
        public struct kResponse
        {
            public string s_Result;
        };
        public cTransfer() { }

        public delegate kResponse del_SlaveCall(kAction k_Action);
        public event del_SlaveCall ev_SlaveCall;

        public kResponse CallSlave(kAction k_Action)
        {
            return ev_SlaveCall(k_Action);
        }

        public override Object InitializeLifetimeService()
        {
            return null;
        }

    }
   
}
