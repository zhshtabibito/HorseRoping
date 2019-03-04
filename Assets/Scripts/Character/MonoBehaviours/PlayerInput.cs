using UnityEngine;

namespace HorseGame
{
    public class PlayerInput : InputComponent
    {
        //public static PlayerInput Instance
        //{
        //    get { return s_Instance; }
        //}

        //protected static PlayerInput s_Instance;


        public bool HaveControl { get { return m_HaveControl; } }

        public int playerNum = 1;
        public InputButton Pause = new InputButton(KeyCode.Escape, XboxControllerButtons.Menu, 1);
        public InputButton ThrowRope = new InputButton(KeyCode.E, XboxControllerButtons.RightBumper, 1);
        public InputButton Dash = new InputButton(KeyCode.Keypad0, XboxControllerButtons.X, 1);
        public InputButton Pull = new InputButton(KeyCode.R, XboxControllerButtons.B, 1);
        public InputAxis PlayerHorizontal = new InputAxis(KeyCode.D, KeyCode.A, XboxControllerAxes.LeftstickHorizontal, 1);
        public InputAxis PlayerVertical = new InputAxis(KeyCode.W, KeyCode.S, XboxControllerAxes.LeftstickVertical, 1);
        public InputAxis AimerHorizontal = new InputAxis(KeyCode.RightArrow, KeyCode.LeftArrow, XboxControllerAxes.RightstickHorizontal, 1);
        public InputAxis AimerVertical = new InputAxis(KeyCode.UpArrow, KeyCode.DownArrow, XboxControllerAxes.RightstickVertical, 1);

        protected bool m_HaveControl = true;

        protected bool m_DebugMenuIsOpen = false;

        void Awake()
        {
            //if (s_Instance == null)
            //    s_Instance = this;
            //else
            //    throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + s_Instance.name + " and " + name + ".");
            SetKeyMap();
        }

        void OnEnable()
        {
            //if (s_Instance == null)
            //    s_Instance = this;
            //else if (s_Instance != this)
            //    throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + s_Instance.name + " and " + name + ".");

            //Application.targetFrameRate = 60;
        }

        void OnDisable()
        {
            //s_Instance = null;
        }

        /// <summary>
        /// Call by Update() in InputComponent.
        /// </summary>
        /// <param name="fixedUpdateHappened"></param>
        protected override void GetInputs(bool fixedUpdateHappened)
        {
            //Debug.Log(fixedUpdateHappened);
            Pause.Get(fixedUpdateHappened, inputType);
            ThrowRope.Get(fixedUpdateHappened, inputType);
            Dash.Get(fixedUpdateHappened, inputType);
            Pull.Get(fixedUpdateHappened, inputType);
            //Jump.Get(fixedUpdateHappened, inputType);
            PlayerHorizontal.Get(inputType);
            PlayerVertical.Get(inputType);
            AimerHorizontal.Get(inputType);
            AimerVertical.Get(inputType);

            if (Input.GetKeyDown(KeyCode.F12))
            {
                m_DebugMenuIsOpen = !m_DebugMenuIsOpen;
            }
        }

        public override void GainControl()
        {
            m_HaveControl = true;

            GainControl(Pause);
            GainControl(ThrowRope);
            GainControl(Dash);
            GainControl(Pull);
            //GainControl(Jump);
            GainControl(PlayerHorizontal);
            GainControl(PlayerVertical);
            GainControl(AimerHorizontal);
            GainControl(AimerVertical);
        }

        public override void ReleaseControl(bool resetValues = true)
        {
            m_HaveControl = false;

            ReleaseControl(Pause, resetValues);
            ReleaseControl(ThrowRope, resetValues);
            ReleaseControl(Dash, resetValues);
            ReleaseControl(Pull, resetValues);
            //ReleaseControl(Jump, resetValues);
            ReleaseControl(PlayerHorizontal, resetValues);
            ReleaseControl(PlayerVertical, resetValues);
            ReleaseControl(AimerHorizontal, resetValues);
            ReleaseControl(AimerVertical, resetValues);
        }

        public void DisableDash()
        {
            Dash.Disable();
        }

        public void EnableDash()
        {
            Dash.Enable();
        }

        public void DisablePull()
        {
            Pull.Disable();
        }

        public void EnablePull()
        {
            Pull.Enable();
        }

        public void SetKeyMap()
        {
            Pause = new InputButton(Pause.key, Pause.controllerButton, playerNum);
            ThrowRope = new InputButton(ThrowRope.key, ThrowRope.controllerButton, playerNum);
            Dash = new InputButton(Dash.key, Dash.controllerButton, playerNum);
            Pull = new InputButton(Pull.key, Pull.controllerButton, playerNum);
            PlayerHorizontal = new InputAxis(PlayerHorizontal.positive, PlayerHorizontal.negative, PlayerHorizontal.controllerAxis, playerNum);
            PlayerVertical = new InputAxis(PlayerVertical.positive, PlayerVertical.negative, PlayerVertical.controllerAxis, playerNum);
            AimerHorizontal = new InputAxis(AimerHorizontal.positive, AimerHorizontal.negative, AimerHorizontal.controllerAxis, playerNum);
            AimerVertical = new InputAxis(AimerVertical.positive, AimerVertical.negative, AimerVertical.controllerAxis, playerNum);
        }
        void OnGUI()
        {
            if (m_DebugMenuIsOpen)
            {
                const float height = 100;

                GUILayout.BeginArea(new Rect(30, Screen.height - height, 200, height));

                GUILayout.BeginVertical("box");
                GUILayout.Label("Press F12 to close");

                bool meleeAttackEnabled = GUILayout.Toggle(Dash.Enabled, "Enable Melee Attack");
                bool rangeAttackEnabled = GUILayout.Toggle(Pull.Enabled, "Enable Range Attack");

                if (meleeAttackEnabled != Dash.Enabled)
                {
                    if (meleeAttackEnabled)
                        Dash.Enable();
                    else
                        Dash.Disable();
                }

                if (rangeAttackEnabled != Pull.Enabled)
                {
                    if (rangeAttackEnabled)
                        Pull.Enable();
                    else
                        Pull.Disable();
                }
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
    }
}
