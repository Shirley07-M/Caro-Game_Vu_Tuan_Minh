using System;
using UnityEngine;
using System.Collections.Generic;

public class CardBoardManager : MonoBehaviour
{
    private Transform caroBoardUI;
    [SerializeField] private CaroController caroPrefab;
    [SerializeField] private int boardSize = 3;
    [SerializeField] private int winLength = 3;
    private CaroState[] caroState;
    private CaroController[] caroBoard;
    private Coord[] directions = { Coord.Down, Coord.Right, Coord.DiagLeft, Coord.DiagRight };
    private CaroState playerSide = CaroState.FilledX;
    private CaroState botSide = CaroState.FilledO;

    private void Awake()
    {
        caroBoardUI = transform;
        caroState = new CaroState[boardSize * boardSize];
        caroBoard = new CaroController[boardSize * boardSize];
    }

    private void Start()
    {
        InitBoard();
    }
    private void InitBoard()
    {
        for (int i = 0; i < boardSize * boardSize; i++)
        {
            CaroController currentCaro = Instantiate(caroPrefab, caroBoardUI);
            currentCaro.SetCaroIndex(i);
            currentCaro.caro.onClick.AddListener(() => HandleCaroClicked(currentCaro.caroindex));

            caroState[i] = CaroState.Empty;
            caroBoard[i] = currentCaro;
        }
    }

    private void HandleCaroClicked(int caroIndex)
    {
        if (caroState[caroIndex] != CaroState.Empty) return;

        // Lượt của người chơi
        caroState[caroIndex] = playerSide;
        caroBoard[caroIndex].SetCaroValue(playerSide);

        Coord playerCoord = Convert1DTo2D(caroIndex);
        if (CheckWin(playerCoord, playerSide))
        {
            CaroEvent.InvokeGameWin();
            DisableBoardInteraction();
            return;
        }

        if (CheckDraw())
        {
            CaroEvent.InvokeGameDraw();
            DisableBoardInteraction();
            return;
        }

        // Lượt của Bot
        int opponentMoveIndex = CaroAI.RandomMove(caroState);
        if (opponentMoveIndex == -1) return;

        caroState[opponentMoveIndex] = botSide;
        caroBoard[opponentMoveIndex].SetCaroValue(botSide);

        Coord botCoord = Convert1DTo2D(opponentMoveIndex);
        if (CheckWin(botCoord, botSide))
        {
            CaroEvent.InvokeGameLost();
            DisableBoardInteraction();
            return;
        }

        if (CheckDraw())
        {
            CaroEvent.InvokeGameDraw();
            DisableBoardInteraction();
            return;
        }
    }

    private bool CheckWin(Coord clickedCoord, CaroState targetSide)
    {
        foreach (Coord dir in directions)
        {
            Coord oppositeDir = (-1) * dir;

            int forwardStreakCount = CountStreak(clickedCoord, dir, targetSide);
            int backwardStreakCount = CountStreak(clickedCoord, oppositeDir, targetSide);

            int totalStreakCount = 1 + forwardStreakCount + backwardStreakCount;

            if (totalStreakCount >= winLength) return true;
        }
        return false;
    }

    private int CountStreak(Coord startCoord, Coord dir, CaroState targetSide)
    {
        int streakCount = 0;

        for (int step = 1; step < winLength; step++)
        {
            Coord nextCoord = startCoord + step * dir;
            if (!IsInsideBoard(nextCoord)) break;

            int nextIndex = Convert2DTo1D(nextCoord);
            if (caroState[nextIndex] != targetSide) break;

            streakCount++;
        }

        return streakCount;
    }

    private bool CheckDraw()
    {
        foreach (var state in caroState)
        {
            if (state is CaroState.Empty) return false;
        }
        return true;
    }

    private void DisableBoardInteraction()
    {
        foreach (CaroController controller in caroBoard)
        {
            controller.ForceUnallowInteract();
        }
    }

    private bool IsInsideBoard(Coord coord)
    {
        if (coord.row >= boardSize || coord.row < 0 || coord.col >= boardSize || coord.col < 0) return false;
        return true;
    }

    private int Convert2DTo1D(Coord coord)
    {
        return coord.row * boardSize + coord.col;
    }

    private Coord Convert1DTo2D(int caroIndex)
    {
        return new Coord(caroIndex / boardSize, caroIndex % boardSize);
    }
}