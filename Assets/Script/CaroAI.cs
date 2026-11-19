using System;
using System.Collections.Generic;
public static class CaroAI //* Cắt đứt liên hệ với unity để chạy multi thread (Nếu dính tới unity thường bị ép chạy main thread)
{
    private static readonly Random random = new();
    public static int RandomMove(CaroState[] boardState)
    {
        List<int> emptyCaroIndex = new();
        for (int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] is CaroState.Empty) emptyCaroIndex.Add(i);
        }

        if (emptyCaroIndex.Count == 0) return -1;

        int randomIndex = random.Next(0, emptyCaroIndex.Count);

        return emptyCaroIndex[randomIndex];
    }

    public static int OptimalMove(CaroState[] boardState, int depth, bool isPlayerTurn, int boardSize, int winLength, Coord[] directions)
    {
        int bestScore = -1000000000;
        int optimalIndex = -1;

        List<int> emptyCaroIndex = new();
        for (int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] is CaroState.Empty) emptyCaroIndex.Add(i);
        }

        if (emptyCaroIndex.Count == 0) return optimalIndex;

        foreach (int index in emptyCaroIndex)
        {
            boardState[index] = CaroState.FilledO;

            int score = MiniMax(boardState, depth + 1, true, boardSize, winLength, directions);

            boardState[index] = CaroState.Empty;

            if (score > bestScore)
            {
                bestScore = score;
                optimalIndex = index;
            }
        }
        return optimalIndex;
    }

    #region Note về depth
    //! depth: độ sâu tầng đệ quy, depth = số bước suy nghĩ của AI
    //! depth càng nhỏ nghĩa là AI nghĩ càng nhanh
    //! Về sau có thể triển khai thêm maxDepth (giới hạn số bước nghĩ trước của AI), nhưng sẽ cần thêm logic Heuristic phức tạp
    //! Ví dụ: maxDepth = 1 -> easy, maxDepth = 4 -> normal, maxDepth tự do -> master
    //! Hiện tại đang cho depth chạy tự do để triển khai thuật toán tính điểm đơn giản: 10 - depth hoặc depth - 10
    #endregion
    #region Note về công thức tính điểm của AI
    //! Nước đi thắng -> Điểm > 0
    //! Nước đi hòa -> Điểm = 0
    //! Nước đi thua -> Điểm < 0

    //! Tại sao lượt của bot lại là (boardSize * boardSize + 1) - depth ?
    //* Bot là maximizer - lấy điểm số to nhất:
    //* Nếu nước cờ thắng -> Thắng càng nhanh càng tốt, không nhây vì người chơi sẽ có cơ hội lật kèo
    //* -> Đặt công thức (boardSize * boardSize + 1) - depth, nghĩa là cứ nhây 1 nước là điểm cũng trừ 1.
    //* Hàm Max đảm bảo rằng bot luôn chọn nước đi điểm cao nhất (điểm cao nhất - nghĩa là nước đi ít nhất - nghĩa là không nhây)
    #endregion
    public static int MiniMax(CaroState[] boardState, int depth, bool isPlayerTurn, int boardSize, int winLength, Coord[] directions)
    {
        List<int> emptyCaroIndex = new();
        for (int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] is CaroState.Empty) emptyCaroIndex.Add(i);
        }

        if (emptyCaroIndex.Count == 0) return 0; //! Nếu hòa

        if (!isPlayerTurn) //! Lượt của bot
        {
            int bestScore = -1000000000;

            foreach (int index in emptyCaroIndex)
            {
                boardState[index] = CaroState.FilledO;
                Coord currentBotCoord = Convert1DTo2D(index, boardSize);

                if (CheckWin(currentBotCoord, CaroState.FilledO, boardState, directions, winLength, boardSize))
                {
                    boardState[index] = CaroState.Empty;
                    return (boardSize * boardSize + 1) - depth;
                }

                int scoreOfPlayer = MiniMax(boardState, depth + 1, true, boardSize, winLength, directions);
                boardState[index] = CaroState.Empty;

                bestScore = Math.Max(scoreOfPlayer, bestScore);
            }
            return bestScore;
        }
        else //! Lượt người chơi (giả định)
        {
            int minScore = 1000000000;

            foreach (int index in emptyCaroIndex)
            {
                boardState[index] = CaroState.FilledX;
                Coord currentPlayerCoord = Convert1DTo2D(index, boardSize);

                if (CheckWin(currentPlayerCoord, CaroState.FilledX, boardState, directions, winLength, boardSize))
                {
                    boardState[index] = CaroState.Empty;
                    return depth - (boardSize * boardSize + 1);
                }

                int scoreOfBot = MiniMax(boardState, depth + 1, false, boardSize, winLength, directions);
                boardState[index] = CaroState.Empty;

                minScore = Math.Min(minScore, scoreOfBot);
            }
            return minScore;
        }
    }
    private static bool CheckWin(Coord clickedCoord, CaroState targetSide, CaroState[] caroState, Coord[] directions, int winLength, int boardSize)
    {
        foreach (Coord dir in directions)
        {
            Coord oppositeDir = (-1) * dir;

            int forwardStreakCount = CountStreak(clickedCoord, dir, targetSide, caroState, winLength, boardSize);
            int backwardStreakCount = CountStreak(clickedCoord, oppositeDir, targetSide, caroState, winLength, boardSize);

            int totalStreakCount = 1 + forwardStreakCount + backwardStreakCount;

            if (totalStreakCount >= winLength) return true;
        }
        return false;
    }

    private static int CountStreak(Coord startCoord, Coord dir, CaroState targetSide, CaroState[] caroState, int winLength, int boardSize)
    {
        int streakCount = 0;

        for (int step = 1; step < winLength; step++)
        {
            Coord nextCoord = startCoord + step * dir;
            if (!IsInsideBoard(nextCoord, boardSize)) break;

            int nextIndex = Convert2DTo1D(nextCoord, boardSize);
            if (caroState[nextIndex] != targetSide) break;

            streakCount++;
        }

        return streakCount;
    }
    private static bool IsInsideBoard(Coord coord, int boardSize)
    {
        if (coord.row >= boardSize || coord.row < 0 || coord.col >= boardSize || coord.col < 0) return false;
        return true;
    }

    private static int Convert2DTo1D(Coord coord, int boardSize)
    {
        return coord.row * boardSize + coord.col;
    }

    private static Coord Convert1DTo2D(int caroIndex, int boardSize)
    {
        return new Coord(caroIndex / boardSize, caroIndex % boardSize);
    }
}