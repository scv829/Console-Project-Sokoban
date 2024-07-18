using System;
using System.IO;

namespace Sokoban
{
    #region 경로의 일부를 찾을 수 없습니다 오류
    // 84줄에 설명 있습니다.
    #endregion


    class Program
    {
        /// <summary>
        /// 맵에 있는 오브젝트들
        /// 0 - 땅
        /// 1 - 벽 
        /// 2 - 박스
        /// 3 - 골인
        /// 4 - 플레이어
        /// 5 - 박스 + 골인
        /// </summary>
        public enum field { Blank, Wall, Box, Goal, Player, Check}

        /// <summary>
        /// 방향키를 눌렀을 때 나타내는 방향
        /// </summary>
        public enum Direction { Up, Down, Left, Right, None }

        /// <summary>
        /// 게임에 대한 정보를 가지고 있는 구조체
        /// </summary>
        public struct gameData
        {
            public bool isExit;         // 게임 종료 여부
            public bool running;        // 게임 클리어 여부

            public int mapNumber;       // 맵의 번호
            public field[,] map;        // 맵의 좌표

            public pos playerPos;       // 플레이어의 시작 좌표

            public int goals;           // 남은 골인 지점

            public ConsoleKey inputKey; // 내가 누른 키
            public Direction direction; // 내가 누른 키의 방향

            public FileInfo[] mapList;  // 맵 리스트

        }

        /// <summary>
        /// 맵 제작에 대한 정보를 가지고 있는 구조체
        /// </summary>
        public struct createMapData
        {
            public field[,] map;        // 원본 맵

            public int boxCount;        // 맵에 있는 박스의 숫자
            public int GoalCount;        // 맵에 있는 골인점의 숫자
   

            public bool isMaking;       // 맵 제작 여부
            public bool flag;           // 테스트 성공 여부
            public bool isTesting;      // 맵 테스팅 여부
            public bool isPlayer;       // 테스팅 맵에 플레이어가 있는지 확인

            public pos cursorPos;       // 현재 에디터 위치
            public pos playerPos;       // 플레이어 위치

        }


        public struct pos
        {
            public int x;
            public int y;
        }

        static gameData data;
        static createMapData testData;



        /*         파일 경로를 설정해야 하는데 사람마다 세팅이 달라서 따로 설정을 해줘야합니다.             */
        /* 아래 경로를 입력하면 Sokoban\bin\Debug\maps 디버그 파일에 저장이 되어서 따로 설정해 줘야합니다.  */
        const string PATH = "./maps";



        // 방향에 따른 위치 변화량이 들어 있는 배열
        static pos[] posChange = new pos[]
        {
                new pos{ x =  0,  y = -1 },     // 상
                new pos{ x =  0,  y =  1 },     // 하
                new pos{ x = -1,  y =  0 },     // 좌
                new pos{ x =  1,  y =  0 },     // 우
                new pos{ x =  0,  y =  0 },     // 이 외 다른 키들
        };


        static void Main(string[] args)
        {

            Start();

            while (!data.isExit)    // 게임 종료를 할 때 까지 반복
            {
                ShowMainMenu();
                while (data.running)// 해당 맵을 클리어할 때 까지 반복
                {
                    Render();
                    Input();
                    Update();
                }
            }

            End();

        }

        static void Start()
        {
            Console.CursorVisible = false;

            data = new gameData();
            data.running = false;

            testData = new createMapData();
            testData.isMaking = false;


            posChange = new pos[]
            {
                new pos{ x =  0,  y = -1 },     // 상
                new pos{ x =  0,  y =  1 },     // 하
                new pos{ x = -1,  y =  0 },     // 좌
                new pos{ x =  1,  y =  0 },     // 우
                new pos{ x =  0,  y =  0 },     // 이 외 다른 키들
            };

            // 파일 입출력으로 맵의 리스트들을 가져옴
            LoadMapList();

            // 시작화면 보이기
            Console.Clear();
            ShowTitle();
            Console.WriteLine("    계속하려면 아무키나 누르세요    ");
            Console.ReadKey();

        }


        /// <summary>
        /// 맵들 불러오기
        /// </summary>
        static void LoadMapList()
        {
            DirectoryInfo maps = new DirectoryInfo(PATH);

            data.mapList = maps.GetFiles("*.txt");
        }

        /// <summary>
        /// 불러온 맵들 출력하기
        /// </summary>
        static void ShowMapList()
        {
            int startPos = 11;
            int index = 0;

            Console.SetCursorPosition(12, 9); Console.Write("──────────── 맵 목록 ────────────");

            foreach (FileInfo map in data.mapList)
            {
                Console.SetCursorPosition(18, startPos + index); Console.Write($"{index + 1}. {map.Name.Replace(".txt", "")}");
                index++;
            }
        }

        /// <summary>
        /// 해당 맵에 대한 맵 데이터 불러오기
        /// </summary>
        /// <param name="index">맵의 고유 번호</param>
        static void LoadMap(int index)
        {
            string line;
            int y = 0;

            try
            {

                StreamReader sr = data.mapList[index].OpenText();


                line = sr.ReadLine();
                string[] playerPos = line.Split(' ');

                // 플레이어 시작 위치 설정
                int.TryParse(playerPos[0], out data.playerPos.x); int.TryParse(playerPos[1], out data.playerPos.y);

                line = sr.ReadLine();
                string[] mapSize = line.Split(' ');

                // 맵의 크기 설정
                int.TryParse(mapSize[0], out int width); int.TryParse(mapSize[1], out int height);

                data.map = new field[height, width];

                line = sr.ReadLine();

                while (line != null)
                {
                    string[] fields = line.Split(' ');

                    for (int x = 0; x < width; x++)
                    {
                        // 맵 배치 불러오기
                        Enum.TryParse(fields[x], out data.map[y, x]);
                    }
                    y++;
                    line = sr.ReadLine();
                }

                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            // 현재 맵 고유 번호와 게임 시작 설정
            data.running = true;
            data.mapNumber = index;

            // 남은 골인 지점 설정
            CountGoals();
        }


        static void ShowTitle()
        {
            Console.WriteLine("===========================================================");
            Console.WriteLine("=     _____         _            _                        =");
            Console.WriteLine("=    /  ___|       | |          | |                       =");
            Console.WriteLine("=    \\ `--.   ___  | | __  ___  | |__    __ _  _ __       =");
            Console.WriteLine("=     `--. \\ / _ \\ | |/ / / _ \\ | '_ \\  / _` || '_ \\      =");
            Console.WriteLine("=    /\\__/ /| (_) ||   < | (_) || |_) || (_| || | | |     =");
            Console.WriteLine("=    \\____/  \\___/ |_|\\_\\ \\___/ |_.__/  \\__,_||_| |_|     =");
            Console.WriteLine("=                                                         =");
            Console.WriteLine("===========================================================");
            Console.WriteLine();
        }

        static void ShowMainMenu()
        {
            int index = 0;
            bool select = false;
            pos cursorPos = new pos { x = 25, y = 15 };
            string[] manuList = new string[]
            {
                "1. 게임 시작",
                "2.  맵  제작",
                "3. 게임 종료"
            };

            while (!select)
            {
                Console.Clear();
                ShowTitle();
                PrintInfo();

                for(int i = 0; i < manuList.Length; i++)
                {
                    Console.SetCursorPosition(cursorPos.x, cursorPos.y + (2 * i)); Console.Write(manuList[i]);
                }

                Console.SetCursorPosition(23, 15 + (2 * index)); Console.Write(">");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        index = (index > 0) ? index - 1 : 0;
                        break;
                    case ConsoleKey.DownArrow:
                        index = (index < manuList.Length - 1) ? index + 1 : manuList.Length - 1;
                        break;
                    case ConsoleKey.Enter:
                        select = true;
                        break;
                }

            }
            if (index == 0) ShowMapMenu();
            else if (index == 1) MakeMap();
            else data.isExit = true;

        }

        static void ShowMapMenu()
        {
            int index = 0;

            while (!data.running)
            {
                Console.Clear();
                ShowTitle();
                PrintInfo();

                ShowMapList();
                Console.SetCursorPosition(16, 11 + index); Console.Write(">");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        index = (index > 0) ? index - 1 : 0;
                        break;
                    case ConsoleKey.DownArrow:
                        index = (index < data.mapList.Length - 1) ? index + 1 : data.mapList.Length - 1;
                        break;
                    case ConsoleKey.R:
                        LoadMapList();
                        break;
                    case ConsoleKey.Enter:
                        LoadMap(index);
                        break;
                    case ConsoleKey.Escape:
                        return;
                }

            }

        }

        /// <summary>
        /// 설명 UI
        /// </summary>
        static void PrintInfo()
        {
            string[] explain;
            int yPos, xPos;

            // 게임 내 
            if (data.running || testData.isTesting)
            {
                string escapeWord = (testData.isTesting) ? "수정 메뉴" : "메인 메뉴";   
                explain = new string[]
                {
                    "       ┌──남은 골인 지점──┐",
                    "       │                  │",
                   $"       │       {data.goals, -2} 개      │",
                    "       │                  │",
                    "       └──────────────────┘",
                    "",
                    "",
                    "",
                    "┌──────────플레이  방법──────────┐",
                    "│                                │",
                    "│    [방향키]                    │",
                    "│       ▲                       │",
                    "│     ◀□▶  : 이동             │",
                    "│       ▼                       │",
                    "│                                │",
                    "│       R     :  다시 하기       │",
                   $"│      ESC    :  {escapeWord}로     │",
                    "│                                │",
                    "└────────────────────────────────┘",
                };
                yPos = 3;
                xPos = data.map.GetLength(1) + 20;
            }
            // 게임 제작
            else if(testData.isMaking)
            {
                explain = new string[]
                {
                    "",
                    "┌───────────────────────유의사항─────────────────────┐",
                    "│   1. 플레이어(P)는  맵에 하나만 가능               │",
                    "│   2. 박스(B)와 골인점(G)와 같아야 테스트 하기 가능 │",
                    "│   3. 박스의 수는 골인점보다 많을 수 없음           │",
                    "│   4. ESC를 누르면 저장하지 않고 메뉴로 이동        │",
                    "│   5. 저장을 하기위해서는 테스트를 통과해야 가능    │",
                    "├─────────────────────맵 제작 방법───────────────────┤",
                    "│                                                    │",
                    "│         [방향키]                                   │",
                    "│            ▲                                      │",
                    "│          ◀□▶  : 에디터(E) 이동                  │",
                    "│            ▼                                      │",
                    "│                                                    │",
                    "│                                                    │",
                    "│               [설치 블럭 타입]                     │",
                    "│            0     :      바닥     (공백)            │",
                    "│            1     :       벽       (#)              │",
                    "│            2     :      박스      (B)              │",
                    "│            3     :     골인점     (G)              │",
                    "│            4     :    플레이어    (P[빨])          │",
                    "│            4     :  박스 + 골인점 (B[파])          │",
                    "│                                                    │",
                    "│            B     :  테스트 하기                    │",
                    "│                                                    │",
                    "│            S     :  저장   하기                    │",
                    "│        [테스트로 클리어하면 저장 가능]             │",
                    "│                                                    │",
                    "│           ESC    :  메인 메뉴로                    │",
                    "│          [저장하지 않고 메뉴로 이동]               │",
                    "│                                                    │",
                    "└────────────────────────────────────────────────────┘",
                };

                if (testData.flag) explain[0] = "저장 가능!";

                yPos = 3;
                xPos = testData.map.GetLength(1) + 20;
            }
            // 게임 외
            else
            {
                explain = new string[]
                {
                    "┌──────────메뉴조작──────────┐",
                    "│                            │",
                    "│     ▲  : 커서 올리기      │",
                    "│     ▼  : 커서 내리기      │",
                    "│                            │",
                    "│    R    :  리스트 새로고침 │",
                    "│    Enter: 해당 메뉴 선택   │",
                    "│    ESC  :  메인 메뉴로     │",
                    "│                            │",
                    "└────────────────────────────┘",
                };
                yPos = 15; xPos = 65;
            }

            for (int i = 0; i < explain.Length; i++)
            {
                Console.SetCursorPosition(xPos, yPos + i);
                Console.Write(explain[i]);
            }
        }

        static void Render()
        {
            Console.Clear();

            PrintMap(data.map);
            PrintInfo();
            PrintPlayer();
        }

        /// <summary>
        /// 맵 UI
        /// </summary>
        static void PrintMap(field[,] map)
        {

            for (int y = 0; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] == field.Blank)
                    {
                        Console.Write(" ");
                    }
                    else if (map[y, x] == field.Wall)
                    {
                        Console.Write("#");
                    }
                    else if (map[y, x] == field.Box)
                    {
                        Console.Write("B");
                    }
                    else if (map[y, x] == field.Goal)
                    {
                        Console.Write("G");
                    }
                    else if (map[y, x] == field.Player) // 맵 제작에서 플레이어 위치를 보기위함
                    {
                        if (testData.isMaking && !testData.isTesting)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("P");
                            Console.ResetColor();
                        }
                        else Console.Write(" ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("B");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
        }

        static void PrintPlayer()
        {
            Console.SetCursorPosition(data.playerPos.x, data.playerPos.y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("P");
            Console.ResetColor();
        }

        static void Input()
        {
            data.inputKey = Console.ReadKey(true).Key;
        }

        static void Update()
        {
            Move();
            CountGoals();
            IsClear();
        }

        static void Move()
        {

            switch (data.inputKey)
            {
                case ConsoleKey.UpArrow:
                    data.direction = Direction.Up;
                    break;
                case ConsoleKey.DownArrow:
                    data.direction = Direction.Down;
                    break;
                case ConsoleKey.LeftArrow:
                    data.direction = Direction.Left;
                    break;
                case ConsoleKey.RightArrow:
                    data.direction = Direction.Right;
                    break;
                case ConsoleKey.R:
                    if (testData.isTesting) CopyMap();
                    else LoadMap(data.mapNumber);
                    break;
                case ConsoleKey.Escape:
                    // 테스팅 모드 시작 -> 수정
                    if (testData.isTesting) testData.isTesting = false;
                    data.running = false;
                    break;
            }

            // 플레이어 위치에서 원하는 방향의 다음 위치를 가져오기
            pos next = GetNextPos(data.playerPos);

            // 플레이어가 해당 방향으로 움직일 수 있으면 위치 변경
            if (IsCanMove(next))
            {
                data.playerPos = next;
                data.direction = Direction.None;
            }

        }

        /// <summary>
        /// 내가 원하는 방향에 있는 다음 좌표
        /// </summary>
        /// <param name="p">현재 있는 좌표</param>
        /// <returns>내가 입력한 방향에 다음 좌표</returns>
        static pos GetNextPos(pos p)
        {
            return new pos()
            {
                x = p.x + posChange[(int)data.direction].x,
                y = p.y + posChange[(int)data.direction].y
            };
        }


        /// <summary>
        /// 움직일 수 있는지 확인하는 함수
        /// </summary>
        /// <param name="next">나가 가고싶은 방향에 있는 다음 좌표</param>
        /// <returns>갈 수 있는지 여부</returns>
        static bool IsCanMove(pos next)
        {
            if (data.map[next.y, next.x] == field.Blank || data.map[next.y, next.x] == field.Goal || data.map[next.y, next.x] == field.Player)
            {
                return true;
            }
            else if (data.map[next.y, next.x] == field.Box || data.map[next.y, next.x] == field.Check)
            {
                // 해당 박스를 접촉함 -> 박스의 같은 방향으로 봤을 때 다음이 있는지 확인
                return IsCanPush(next);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 움직이는 곳에 박스가 있는데 밀 수 있는지 확인하는 함수
        /// </summary>
        /// <param name="box">박스가 있는 좌표</param>
        /// <returns>박스를 밀어서 내가 움직일 수 있는지 여부</returns>
        static bool IsCanPush(pos box)
        {
            // 박스의 다음 위치 얻기
            pos boxNext = GetNextPos(box);

            if (data.map[boxNext.y, boxNext.x] == field.Blank)  // 비어있다 -> 박스 밀기 가능
            {
                // 골인에 있는 상자를 밀어서 가는지 확인
                data.map[box.y, box.x] = data.map[box.y, box.x] == field.Check ? field.Goal : field.Blank;
                data.map[boxNext.y, boxNext.x] = field.Box;
                return true;
            }
            else if (data.map[boxNext.y, boxNext.x] == field.Goal) // 골인 지점이다 -> 해당 골인 지점에 박스를 놓아서 밀기 가능
            {
                // 골인 지점으로 박스를 밀어서 체크하고, 원래 박스 위치는 비어있게
                data.map[box.y, box.x] = data.map[box.y, box.x] == field.Check ? field.Goal : field.Blank;
                data.map[boxNext.y, boxNext.x] = field.Check;

                return true;
            }
            else // 안되는 상황 1. 박스 뒤에 박스가 있을 때 2. 박스 뒤에 벽이 있을 때
            {
                return false;
            }

        }

        /// <summary>
        /// 남아 있는 골인 지점이 몇개인지 확인하는 함수
        /// </summary>
        static void CountGoals()
        {
            int goals = 0;
            foreach (field check in data.map)
            {
                if (check == field.Goal) goals += 1;
            }

            data.goals = goals;
        }

        static void IsClear()
        {

            if (data.goals == 0)
            {
                // 맵 제작 테스팅 모드일 때
                if (testData.isTesting)
                {
                    testData.flag = true;
                    testData.isTesting = false;
                    return;
                }

                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("=                                  =");
                Console.WriteLine("=           게임 클리어!           =");
                Console.WriteLine("=                                  =");
                Console.WriteLine("====================================");
                Console.WriteLine();
                Console.WriteLine("    메인 메뉴로 돌아가려면 아무키나 누르세요    ");
                Console.ReadKey();

                data.running = false;
            }
        }


        static void MakeMap()
        {
            testData.isMaking = true;
            testData.flag = false;
            testData.cursorPos = new pos() { x = 1, y = 1 };
            testData.isPlayer = false;

            // 1. 입력 배열 크기
            Console.Clear();
            Console.WriteLine("가로 세로 길이를 입력해주세요");

            Console.Write("가로 길이 : "); int.TryParse(Console.ReadLine(), out int width);
            Console.Write("세로 길이 : "); int.TryParse(Console.ReadLine(), out int height);
    

            width += 2; height += 2;

            testData.map = new field[height, width];
            FillMap();

            // 2. 멥 그리기, (1,1) 위치로 커서 설정
            Console.SetCursorPosition(1, 1);

            // 3. 방향키로 움직이는 데 해당 자리는 키넘버 or 다이얼 키로 눌러서 커서 위치에 배치
            while (testData.isMaking)
            {
                Console.Clear();
                PrintMap(testData.map);
                PrintInfo();
                PrintEditer();
                CountObejct();

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        data.direction = Direction.Up;
                        break;
                    case ConsoleKey.DownArrow:
                        data.direction = Direction.Down;
                        break;
                    case ConsoleKey.LeftArrow:
                        data.direction = Direction.Left;
                        break;
                    case ConsoleKey.RightArrow:
                        data.direction = Direction.Right;
                        break;
                    // 4. 종류는 field에 맞게  0 - 땅, 1 - 벽, 2 - 박스, 3 - 골인, 4 - 플레이어, 5 - 박스 + 골인 로 배치
                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                        InsertMap(field.Blank);
                        break;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        InsertMap(field.Wall);
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        InsertMap(field.Box);
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        InsertMap(field.Goal);
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        InsertMap(field.Player);
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        InsertMap(field.Check);
                        break;
                    // 5. B 버튼으로 시작, R버튼으로 초기화, esc버튼으로 시작 -> 수정,  수정 -> 종료
                    case ConsoleKey.B:
                        // 시작 - 골인점과 박스의 갯수가 동일해야 함
                        if(testData.boxCount == testData.GoalCount && testData.isPlayer)
                        {
                            testData.isTesting = true;
                            TestingMap();
                        }
                        break;
                    // 6. 시작 했을 때 성공했다 -> 저장가능(S 버튼 활성화)
                    case ConsoleKey.S:
                        // 저장
                        if (testData.flag)
                        {
                            testData.isMaking = false;
                            SaveMap();
                        }
                        break;
                    case ConsoleKey.Escape:
                        // 수정 -> 종료
                        testData.isMaking = false;
                        break;
                }

                pos next = GetNextPos(testData.cursorPos);

                
                if( 0 < next.x && next.x < width - 1 && 0 < next.y && next.y < height - 1 )
                {
                    testData.cursorPos = next;
                    data.direction = Direction.None;
                }

            }

        }

        private static void FillMap()
        {
            for(int y = 0; y < testData.map.GetLength(0); y++)
            {
                for(int x = 0; x < testData.map.GetLength(1); x++)
                {
                    if (x == 0 || x == testData.map.GetLength(1) - 1) testData.map[y, x] = field.Wall;
                    else if (y == 0 || y == testData.map.GetLength(0) - 1) testData.map[y, x] = field.Wall;
                    else testData.map[y, x] = field.Blank;
                }
            }
        }

        /// <summary>
        /// 에디터의 위치를 알려주는 함수
        /// </summary>
        static void PrintEditer()
        {
            Console.SetCursorPosition(testData.cursorPos.x, testData.cursorPos.y);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("E");
            Console.ResetColor();
        }

        /// <summary>
        /// 맵에 오브젝트를 넣어주는 함수
        /// </summary>
        /// <param name="type"></param>
        static void InsertMap(field type)
        {

            switch (type)
            {
                case field.Blank:
                case field.Wall:
                case field.Goal:
                    testData.map[testData.cursorPos.y, testData.cursorPos.x] = type;
                    break;
                case field.Box:
                    if(testData.boxCount >= testData.GoalCount) return;       // 골인점보다 박스가 많을 수 없다
                    break;
                case field.Player:
                    if (testData.isPlayer) return;                           // 플레이어는 1명 뿐이다
                    break;
            }

            testData.map[testData.cursorPos.y, testData.cursorPos.x] = type;

            if (type == field.Player) // 플레이어가 없으면
            {
                testData.playerPos = testData.cursorPos;
            }

        }

        /// <summary>
        /// 현재 몇개의 오브젝트가 들어있는지 
        /// </summary>
        static void CountObejct()
        {
            int goals = 0;
            int box = 0;
            testData.isPlayer = false;
            foreach (field check in testData.map)
            {
                if (check == field.Goal) goals += 1;
                else if (check == field.Player) testData.isPlayer = true; 
                else if (check == field.Box) box += 1;
            }

            testData.boxCount = box;
            testData.GoalCount = goals;
        }

       
        
           /// <summary>
           /// 맵 테스트할 때 작동하는 메소드
           /// </summary>
        static void TestingMap()
        {
            Console.CursorVisible = false; // 커서가 갑자기 보일 때가 있어서 막기 위한 코드
            CopyMap();                     // 게임 로직을 사용하기 위해 사용 기존 게임 맵 배열에 테스트 배열을 복사하여 기존 함수 사용

            while (testData.isTesting)
            {
                Render();
                Input();
                Update();
            }
        }

        /// <summary>
        /// 기존 맵 <- 테스트 맵
        /// </summary>
        private static void CopyMap()
        {
            data.map = new field[testData.map.GetLength(0), testData.map.GetLength(1)];

            for (int y = 0; y < testData.map.GetLength(0); y++)
            {
                for (int x = 0; x < testData.map.GetLength(1); x++)
                {
                    data.map[y, x] = testData.map[y,x];
                }
            }

            data.playerPos = testData.playerPos;
        }

        /// <summary>
        /// 맵을 저장하는 함수
        /// </summary>
        static void SaveMap()
        {

            // 저장 후 안내 문구
            Console.Clear();
            Console.Write("저장할 맵 제목을 입력하세요 : ");
            string mapName = Console.ReadLine();
            string isOk;
            while (true)
            {
                Console.WriteLine($"[{mapName}] 해당 이름으로 저장하겠습니까?(y/n)");
                isOk =  Console.ReadLine();
                if (isOk.Equals("y")) break;
                else
                {
                    Console.Clear();
                    Console.Write("저장할 맵 제목을 입력하세요 : ");
                    mapName = Console.ReadLine();
                }
            }

            try
            {
                string savePath = PATH + "/" + mapName + ".txt";
                StreamWriter sw = new StreamWriter(savePath);

                // 첫번째 줄에는 플레이어 위치
                sw.WriteLine($"{testData.playerPos.x} {testData.playerPos.y}");
                // 두번째 줄에는 맵의 크기
                sw.WriteLine($"{testData.map.GetLength(0)} {testData.map.GetLength(1)}");
                // 세번째 부터는 배열 한줄 씩 저장
                for (int y = 0; y < testData.map.GetLength(0); y++)
                {
                    for (int x = 0; x < testData.map.GetLength(1); x++)
                    {
                        sw.Write((int)testData.map[y, x] + " ");
                    }
                    sw.WriteLine();
                }

                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("=                                  =");
            Console.WriteLine("=           맵 저장 완료!          =");
            Console.WriteLine("=                                  =");
            Console.WriteLine("====================================");
            Console.WriteLine();
            Console.WriteLine("    메인 메뉴로 돌아가려면 아무키나 누르세요    ");
            Console.ReadKey();
        }



        static void End()
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine("=                                  =");
            Console.WriteLine("=             게임 종료            =");
            Console.WriteLine("=                                  =");
            Console.WriteLine("====================================");
            Console.WriteLine();

        }

    }
}
