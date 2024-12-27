
using System.Collections.Generic;
using IAConfig.Excel.GenCode.Property;
using IAUI;
using IAUI;
using System;
using Gameplay.Map;
using UnityEngine;
using Gameplay;

namespace IAConfig.Excel.Export
{
    internal partial class ExcelExportModule
    {
        
        private void Export_UIPanelCfg(GenConfigInfo pGenInfo,List<BaseProperty> pProps,List<Dictionary<string, List<string>>> propValuelist)
        {
            List<UIPanelCfg> cnfs = new List<UIPanelCfg>();
            foreach (var propDict in propValuelist)
            {
                UIPanelCfg cnf = new UIPanelCfg();
				cnf.id = (UIPanelDef)GetProp(pProps,"id").Parse(propDict["id"][0]);
				cnf.prefab = (string)GetProp(pProps,"prefab").Parse(propDict["prefab"][0]);
				cnf.script = (string)GetProp(pProps,"script").Parse(propDict["script"][0]);
				cnf.layer = (UILayer)GetProp(pProps,"layer").Parse(propDict["layer"][0]);
				cnf.canvas = (UICanvasType)GetProp(pProps,"canvas").Parse(propDict["canvas"][0]);
				cnf.showRule = (UIShowRule)GetProp(pProps,"showRule").Parse(propDict["showRule"][0]);

                cnfs.Add(cnf);
            }
                
            pGenInfo.SaveJson(cnfs);    
        }

        private void Export_MapCfg(GenConfigInfo pGenInfo,List<BaseProperty> pProps,List<Dictionary<string, List<string>>> propValuelist)
        {
            List<MapCfg> cnfs = new List<MapCfg>();
            foreach (var propDict in propValuelist)
            {
                MapCfg cnf = new MapCfg();
				cnf.id = (int)GetProp(pProps,"id").Parse(propDict["id"][0]);
				cnf.name = (string)GetProp(pProps,"name").Parse(propDict["name"][0]);
				cnf.prefab = (string)GetProp(pProps,"prefab").Parse(propDict["prefab"][0]);
				cnf.gridSize = (Vector2)GetProp(pProps,"gridSize").Parse(propDict["gridSize"][0]);

                cnfs.Add(cnf);
            }
                
            pGenInfo.SaveJson(cnfs);    
        }

        private void Export_ActorCfg(GenConfigInfo pGenInfo,List<BaseProperty> pProps,List<Dictionary<string, List<string>>> propValuelist)
        {
            List<ActorCfg> cnfs = new List<ActorCfg>();
            foreach (var propDict in propValuelist)
            {
                ActorCfg cnf = new ActorCfg();
				cnf.id = (int)GetProp(pProps,"id").Parse(propDict["id"][0]);
				cnf.name = (string)GetProp(pProps,"name").Parse(propDict["name"][0]);
				cnf.img = (string)GetProp(pProps,"img").Parse(propDict["img"][0]);
				cnf.prefab = (string)GetProp(pProps,"prefab").Parse(propDict["prefab"][0]);
				cnf.baseSpeed = (float)GetProp(pProps,"baseSpeed").Parse(propDict["baseSpeed"][0]);

                cnfs.Add(cnf);
            }
                
            pGenInfo.SaveJson(cnfs);    
        }

        private void Export_GameLevelCfg(GenConfigInfo pGenInfo,List<BaseProperty> pProps,List<Dictionary<string, List<string>>> propValuelist)
        {
            List<GameLevelCfg> cnfs = new List<GameLevelCfg>();
            foreach (var propDict in propValuelist)
            {
                GameLevelCfg cnf = new GameLevelCfg();
				cnf.id = (int)GetProp(pProps,"id").Parse(propDict["id"][0]);
				cnf.name = (string)GetProp(pProps,"name").Parse(propDict["name"][0]);
				cnf.mapId = (int)GetProp(pProps,"mapId").Parse(propDict["mapId"][0]);
				cnf.type = (int)GetProp(pProps,"type").Parse(propDict["type"][0]);
				cnf.time = (int)GetProp(pProps,"time").Parse(propDict["time"][0]);
				cnf.nextLevel = (int)GetProp(pProps,"nextLevel").Parse(propDict["nextLevel"][0]);

                cnfs.Add(cnf);
            }
                
            pGenInfo.SaveJson(cnfs);    
        }

        private void Export_MiscCfg(GenConfigInfo pGenInfo,List<BaseProperty> pProps,List<Dictionary<string, List<string>>> propValuelist)
        {
            List<MiscCfg> cnfs = new List<MiscCfg>();
            foreach (var propDict in propValuelist)
            {
                MiscCfg cnf = new MiscCfg();
				cnf.name = (string)GetProp(pProps,"name").Parse(propDict["name"][0]);
				cnf.value = (string)GetProp(pProps,"value").Parse(propDict["value"][0]);

                cnfs.Add(cnf);
            }
                
            pGenInfo.SaveJson(cnfs);    
        }

        private void Export_FightDrumsMusicCfg(GenConfigInfo pGenInfo,List<BaseProperty> pProps,List<Dictionary<string, List<string>>> propValuelist)
        {
            List<FightDrumsMusicCfg> cnfs = new List<FightDrumsMusicCfg>();
            foreach (var propDict in propValuelist)
            {
                FightDrumsMusicCfg cnf = new FightDrumsMusicCfg();
				cnf.name = (string)GetProp(pProps,"name").Parse(propDict["name"][0]);
				cnf.res = (string)GetProp(pProps,"res").Parse(propDict["res"][0]);
				cnf.bpm = (float)GetProp(pProps,"bpm").Parse(propDict["bpm"][0]);

                cnfs.Add(cnf);
            }
                
            pGenInfo.SaveJson(cnfs);    
        }

        public ExcelExportModule()
        {
			exportFuncDict.Add("UIPanelCfg",Export_UIPanelCfg);
			exportFuncDict.Add("MapCfg",Export_MapCfg);
			exportFuncDict.Add("ActorCfg",Export_ActorCfg);
			exportFuncDict.Add("GameLevelCfg",Export_GameLevelCfg);
			exportFuncDict.Add("MiscCfg",Export_MiscCfg);
			exportFuncDict.Add("FightDrumsMusicCfg",Export_FightDrumsMusicCfg);

        }
    } 
}
