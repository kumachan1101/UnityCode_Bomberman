using UnityEngine;

namespace BomPosName{
    public class BomPos
    {
        private Vector3 v3Pos;
        public void SetPos(Vector3 v3){
            v3Pos = v3;
        }

        public Vector3 GetPos(){
            return v3Pos;
        }

    }
}
