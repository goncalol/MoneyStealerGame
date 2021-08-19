using UnityEngine;

namespace Assets.Scripts
{
    public class FollowingPoints
    {
        public Transform Position { get; set; }
        public string FollowerNameReference { get; set; }

        public static FollowingPoints Create(Transform Position)
        {
            return new FollowingPoints
            {
                Position = Position
            };
        }
    }
}
