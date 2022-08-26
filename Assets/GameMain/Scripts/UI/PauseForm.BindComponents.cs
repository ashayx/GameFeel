using UnityEngine;
using UnityEngine.UI;
using TMPro;

//自动生成于：7/15/2022 7:09:47 PM
namespace Game
{

	public partial class PauseForm
	{

		private Button m_Button_Settings;
		private Button m_Button_Continue;
		private Button m_Button_Restart;
		private Button m_Button_GiveUp;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Button_Settings = autoBindTool.GetBindComponent<Button>(0);
			m_Button_Continue = autoBindTool.GetBindComponent<Button>(1);
			m_Button_Restart = autoBindTool.GetBindComponent<Button>(2);
			m_Button_GiveUp = autoBindTool.GetBindComponent<Button>(3);
		}
	}
}
