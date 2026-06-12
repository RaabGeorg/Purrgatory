    using UnityEngine;

    public class PauseLogic : MonoBehaviour
    {
        public static bool isPaused { get; set; }
        public static string who;

        public static bool PauseGame(string who2)
        {
            if (!isPaused && who == null)
            {
                who = who2.ToLower();
                SetPaused(true);
                return true;
            }
            else if (who.Equals(who2.ToLower()))
            {
                SetPaused(false);
                who = null;
                return true;
            }

            return false;
        }

        public static void SetPaused(bool paused)
        {
            isPaused = paused;
            Time.timeScale = paused ? 0 : 1;
        }
    }

