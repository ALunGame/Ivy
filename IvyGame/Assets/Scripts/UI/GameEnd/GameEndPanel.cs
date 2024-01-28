using Game.Network.Client;
using Gameplay;
using Gameplay.Player;
using IAUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    internal class GameEndPanel_Model : UIModel
    {

    }

    internal class GameEndPanel : UIPanel<FightPanel_Model>
    {
        public override void OnShow()
        {
            base.OnShow();
        }


    }
}
