using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpp2019_haifischflosse
{
    public static class FilePath
    {
        //main file path
        private const string assets = "assets/";
        private const string saveFiles = "savefiles/";

        //assets sub-folders
        private const string audio = assets + "audio/";
        private const string spritesheet = assets + "spritesheet/";
        private const string texture = assets + "texture/";

        //audio sub-folders
        //private const string none = audioFolder + "none/";

        //spritesheet sub folders
        private const string knightFolder = spritesheet + "knight/";
        private const string mainPlayer = spritesheet + "main_player/";
        private const string projectile = spritesheet + "projectile/"; 

        //texture sub-folders
        private const string background = texture + "backgrounds/";

        //file type
        private const string png = ".png";
        private const string txt = ".txt";
        private const string mp3 = ".mp3";
        private const string wav = ".wav";

        // create your needed file paths here
        // Player
        public const string PlayerIdle = mainPlayer + "adventurer-idle" + png;
        public const string PlayerCrouch = mainPlayer + "adventurer-crouch" + png;
        public const string PlayerCrouchWalk = mainPlayer + "adventurer-crouch-walk" + png; 
        public const string PlayerWalk = mainPlayer + "adventurer-walk" + png;
        public const string PlayerRun = mainPlayer + "adventurer-run" + png;
        public const string PlayerSlide = mainPlayer + "adventurer-slide" + png;
        public const string PlayerShootBow = mainPlayer + "adventurer-bow" + png;
        public const string PlayerJump = mainPlayer + "adventurer-jump" + png;
        public const string PlayerAirShootBow = mainPlayer + "adventurer-bow-jump" + png;
        public const string PlayerAirMeleeAttackStart = mainPlayer + "adventurer-air-attack3-rdy" + png;
        public const string PlayerAirMeleeAttackLoop = mainPlayer + "adventurer-air-attack3-loop" + png;
        public const string PlayerAirMeleeAttackEnd = mainPlayer + "adventurer-air-attack-3-end" + png;
        public const string PlayerAirFall = mainPlayer + "adventurer-fall" + png;
        public const string PlayerRoll = mainPlayer + "adventurer-smrslt" + png;
        public const string PlayerStandUp = mainPlayer + "adventurer-stand" + png;
        public const string PlayerDie = mainPlayer + "adventurer-die" + png; // adventurer-knock-dwn
        // PlayerBullet
        public const string ArrowRotation = projectile + "arrow-rotation" + png;
        public const string ArrowHit = projectile + "arrow-hit" + png;
        public const string ArrowPuff = projectile + "arrow-puff" + png;
        //--- Testing ---
        public const string KnightAttack = knightFolder + "KnightAttack_strip" + png;
        public const string KnightDeath = knightFolder + "KnightDeath_strip" + png;
        public const string KnightIdle = knightFolder + "KnightIdle_strip" + png;
        public const string KnightJumpAndFall = knightFolder + "KnightJumpAndFall_strip" + png;
        public const string KnightRoll = knightFolder + "KnightRoll_strip" + png;
        public const string KnightRun = knightFolder + "KnightRun_strip" + png;
        public const string KnightShield = knightFolder + "KnightShield_strip" + png;

        public const string Coin = spritesheet + "coin" + png;
        public const string Flag = spritesheet + "flag_animation" + png;
        public const string WavingFlag = spritesheet + "spr_flags_strip6" + png;
        public const string Enemy = spritesheet + "enemy" + png;
        public const string Flash = spritesheet + "flash" + png;

        public const string mapLevel1 = saveFiles + "level1" + txt;
        public const string mapLevel2 = saveFiles + "level2" + txt;
        public const string gameObjects = saveFiles + "GameObjects" + txt;
        public const string level1_go = saveFiles + "level1_go" + txt;
        public const string playerSafe = saveFiles + "PlayerSafeFile" + txt;
        // background
        public const string MapBackground = background + "NewBackground2048x1536" + png;
        // music
        public const string GrasslandsTheme = audio + "GrasslandsTheme" + mp3;
        public const string GrayTrip = audio + "GrayTrip" + mp3;
        //sounds
        public const string ButtonClick = audio + "button_click" + mp3;
        public const string Impact = audio + "impact" + wav;
        public const string CoinPickup = audio + "coin_pickup" + mp3; 
        public const string ArrowShoot = audio + "arrow_shoot" + mp3;
        public const string ArrowPlop = audio + "arrow_plop" + mp3;
        public const string ArrowHitExplosion = audio + "arrow_hit_explosion" + mp3;
    }
}
