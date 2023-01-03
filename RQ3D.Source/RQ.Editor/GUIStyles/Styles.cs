using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Editor.GUIStyles
{
    public class Styles
    {
        public static GUIStyle CreateInspectorStyle()
        {
            GUIStyle inspectorBG = new GUIStyle();
            inspectorBG.name = "inspectorBG";
            var tex = Resources.Load<Texture2D>("tk2dSkin/inspectorbg");
            inspectorBG.normal.background = tex;
            inspectorBG.border = new RectOffset(10, 3, 3, 0);
            inspectorBG.padding = new RectOffset(0, 5, 0, 0);
            inspectorBG.overflow = new RectOffset(7, 0, 0, 0);
            return inspectorBG;
        }

        public static GUIStyle CreateInspectorHeaderStyle()
        {
            GUIStyle inspectorBG = new GUIStyle();
            inspectorBG.name = "InspectorHeaderBG";
            var tex = Resources.Load<Texture2D>("tk2dSkin/inspectorheader");
            inspectorBG.normal.background = tex;
            inspectorBG.border = new RectOffset(9, 3, 3, 4);
            inspectorBG.padding = new RectOffset(0, 5, 0, 5);
            inspectorBG.overflow = new RectOffset(7, 0, 0, 0);
            return inspectorBG;
        }

        public static GUIStyle CreateSimpleButtonTemplateStyle()
        {
            GUIStyle simpleButtonTemplate = new GUIStyle();
            simpleButtonTemplate.name = "SimpleButtonTemplate";
            simpleButtonTemplate.fixedHeight = 17;
            simpleButtonTemplate.fixedWidth = 17;
            simpleButtonTemplate.margin.right = 2;
            return simpleButtonTemplate;
        }

        public static GUIStyle CreateTilemapDeleteItemStyle()
        {
            GUIStyle tilemapDeleteItem = new GUIStyle();
            tilemapDeleteItem.name = "TilemapDeleteItem";
            tilemapDeleteItem.normal.background = GetTexture("btn_delete_item");
            tilemapDeleteItem.active.background = GetTexture("btn_delete_item_down");
            tilemapDeleteItem.fixedHeight = 17;
            tilemapDeleteItem.fixedWidth = 17;
            tilemapDeleteItem.margin.right = 2;
            return tilemapDeleteItem;
        }

        public static Texture2D GetTexture(string name)
        {
            Texture2D tex = null;

            tex = Resources.Load<Texture2D>("tk2dSkin/" + name);
            if (tex == null)
            {
                Debug.LogError("tk2d - Cant find skin texture " + name);
                return GetTexture("white");
            }

            return tex;
        }
    }
}
