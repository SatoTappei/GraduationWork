using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class Blueprint
    {
        public static int Height => Base.Length;
        public static int Width => Base[0].Length;

        public static readonly string[] Base =
        {
            "########################",
            "#_#___#___#____________#",
            "#___________##########_#",
            "#_#___#___#_#___#____#_#",
            "#_#########_____#______#",
            "#_#______##_#___#____#_#",
            "#_#______##_##########_#",
            "#_#_####_#___#_______#_#",
            "#______________#_#_#___#",
            "##_#######___#_______#_#",
            "#________##_#####_####_#",
            "#_######_#_____#___#___#",
            "#_#____#_#_____#___#_###",
            "#_#_#__#_##_####___#___#",
            "#_#_####_____###___###_#",
            "#________###___________#",
            "####_############_####_#",
            "#__#_#__#__####_____##_#",
            "#____#__#__###_______#_#",
            "####_##_##_###_________#",
            "#__#_______###_______#_#",
            "#____##########_____####",
            "########################"
        };

        public static readonly string[] Doors =
        {
            "########################",
            "#_#___#___#____________#",
            "#_6___6___6_##########_#",
            "#_#___#___#_#___#____#_#",
            "#_#########_4___#____6_#",
            "#_#______##_#___#____#_#",
            "#_#______##_##########_#",
            "#_#2####2#___#_______#_#",
            "#____________4_#_#_#_6_#",
            "##_#######___#_______#_#",
            "#________##8#####8####_#",
            "#_######_#_____#___#___#",
            "#_#____#_#_____#___#_###",
            "#_#_#__#_##2####___#___#",
            "#_#_####_____###___###_#",
            "#________###___4___6___#",
            "####8############_####_#",
            "#__#_#__#__####_____##_#",
            "#____#__#__###_______#_#",
            "####_##_##_###_______6_#",
            "#__#_______###_______#_#",
            "#____##########_____####",
            "########################"
        };

        public static readonly string[] Treasures =
        {
            "########################",
            "#_#_8_#___#____________#",
            "#___________##########_#",
            "#_#___#___#_#___#____#_#",
            "#_#########____4#______#",
            "#_#______##_#___#____#_#",
            "#_#______##_##########_#",
            "#_#_####_#___#_______#_#",
            "#______________#_#_#___#",
            "##_#######___#_______#_#",
            "#________##_#####_####_#",
            "#_######_#_____#___#___#",
            "#_#____#_#_____#___#_###",
            "#_#_#22#_##_####__4#___#",
            "#_#_####_____###___###_#",
            "#________###___________#",
            "####_############_####_#",
            "#__#_#__#__####_____##_#",
            "#____#__#__###_______#_#",
            "####_##_##_###_________#",
            "#8_#_______###_______#_#",
            "#____##########_____####",
            "########################"
        };


        public static readonly string[] Pillars =
        {
            "########################",
            "#_#___#___#____________#",
            "#___________##########_#",
            "#_#___#___#_#___#____#_#",
            "#_#########_____#______#",
            "#_#______##_#___#____#_#",
            "#_#______##_##########_#",
            "#_#_####_#3_9#_______#_#",
            "#______________#_#_#___#",
            "##_#######1_7#_______#_#",
            "#________##_#####_####_#",
            "#_######_#_____#___#___#",
            "#_#____#_#_____#___#_###",
            "#_#_#__#_##_####___#___#",
            "#_#_####_____###___###_#",
            "#________###___________#",
            "####_############_####_#",
            "#__#_#__#__####_____##_#",
            "#____#__#__###_______#_#",
            "####_##_##_###_________#",
            "#__#_______###_______#_#",
            "#____##########_____####",
            "########################"
        };

        public static readonly string[] Lamp =
        {
            "########################",
            "#8#___#_8_#_____8______#",
            "#___________##########_#",
            "#_#_2_#___#_#___#____#_#",
            "#_#########____6#______#",
            "#_#______##_#___#____#_#",
            "#_#_2__2_##_##########6#",
            "#_#_####_#__6#___2___#_#",
            "#______________#_#_#___#",
            "##4#######4__#___8___#_#",
            "#_______6##_#####_####_#",
            "#_######_#_____#___#___#",
            "#_#4___#_#_____#___#6###",
            "#_#_#__#_##_####___#___#",
            "#_#_####_____###___###_#",
            "#____2___###4_________6#",
            "####_############_####_#",
            "#__#_#__#__####_____##_#",
            "#____#__#__###_______#_#",
            "####6##_##_###_________#",
            "#__#____2__###_______#2#",
            "#____##########_____####",
            "########################"
        };

        // <:����, h:��, k:�G(Kaduki)�̗N���ʒu, B:�^��, C:�R���e�i
        public static readonly string[] Interaction =
        {
            "########################",
            "#_#B__#___#____________#",
            "#_______h___##########_#",
            "#_#___#___#_#__C#___C#_#",
            "#_#########_____#______#",
            "#_#____BB##_#___#CB__#_#",
            "#_#______##_##########_#",
            "#_#_####_#___#_______#_#",
            "#__________<___#_#h#___#",
            "##_#######___#_______#_#",
            "#________##_#####_####_#",
            "#_######_#____B#___#___#",
            "#_#____#_#_____#___#_###",
            "#_#_#__#_##_####C__#___#",
            "#_#_####_____###C__###_#",
            "#________###___________#",
            "####_############_####_#",
            "#__#_#_h#_C####_____##_#",
            "#____#__#__###_______#_#",
            "####_##_##_###_________#",
            "#__#_______###_______#h#",
            "#____##########_____####",
            "########################"
        };
    }
}