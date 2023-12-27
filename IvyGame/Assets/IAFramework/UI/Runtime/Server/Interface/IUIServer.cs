namespace LCUI
{
    public interface IUIServer
    {
        void Init();

        void Clear();

        T GetPanelModel<T>(UIPanelDef panelId) where T : UIModel;

        void Show(UIPanelDef panelId);

        void Hide(UIPanelDef panelId);

        void HideAllActivePanel();

        void DestroyAllPanel();
    }
}
