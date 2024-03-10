﻿using System.Collections.Generic;
using System.Linq;

namespace LSystem
{
    /// <summary>
    /// ----------------------------------------------------- <br/>
    /// * 뼈대 골격의 포즈클래스 <br/>
    /// ----------------------------------------------------- <br/>
    /// - 골격이 원점으로부터 SRT한 행렬을 가진다. <br/>
    /// - 뼈대의 로컬공간의 행렬이 아니다.<br/>
    /// - 골격의 뼈대의 정보는 뼈대이름의 딕셔너리로 접근한다. <br/>
    /// ----------------------------------------------------- <br/>
    /// </summary>
    public class ArmaturePose
    {
        Dictionary<string, BonePose> _pose;

        public ArmaturePose()
        {
            _pose = new Dictionary<string, BonePose>();
        }

        public BonePose this[string jointName]
        {
            get => _pose[jointName];
            set => _pose[jointName] = value;
        }

        public string[] JointNames => _pose.Keys.ToArray();

    }
}
