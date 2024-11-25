using System;
using Controllers.Controller;
using MySingelton;
using UnityEngine;

namespace Controllers
{
    public class DifficultyController : MonoBehaviour
    {
        public float SpeedScale => Singelton.Instance.SpeedController.Speed / 25f;
        public float Change => (Singelton.Instance.SpeedController.Speed - 25f) / (Singelton.Instance.SpeedController.TargetSpeed - 25f);
    }
}
