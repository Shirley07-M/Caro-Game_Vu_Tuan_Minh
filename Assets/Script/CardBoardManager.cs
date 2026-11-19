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
            currentCaro.caro.onClick.AddListener(() => OnCaroClicked(currentCaro.caroindex));

            caroState[i] = CaroState.Empty;
            caroBoard[i] = currentCaro;
        }
    }

    private void OnCaroClicked(int caroIndex)
    {
        if (caroState[caroIndex] != CaroState.Empty) return;

        caroState[caroIndex] = CaroState.FilledX;
        caroBoard[caroIndex].SetCaroValue(CaroState.FilledX);

        if(CheckWin())
        {
            CaroEvent.InvokeGameWin();
            foreach (CaroController controller in caroBoard) controller.ForceUnallowInteract();
            return;
        }
        if(CheckDraw())
        {
            CaroEvent.InvokeGameDraw();
            foreach (CaroController controller in caroBoard) controller.ForceUnallowInteract();
            return;
        }

        int opponentMoveIndex = CaroAI.RandomMove(caroState);
        caroState[opponentMoveIndex] = CaroState.FilledO;
        caroBoard[opponentMoveIndex].SetCaroValue(CaroState.FilledO);
    }
    private bool CheckWin()
    {
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                Coord currentCoord = new Coord(r, c);
                int current1D = Convert2DTo1D(currentCoord);

                if (caroState[current1D] is CaroState.Empty) continue;

                foreach(var dir in directions)
                {
                    if (CheckStreak(currentCoord, dir, current1D)) return true;
                }
            }
        }
        return false;
    }

    private bool CheckStreak(Coord currentCoord, Coord dir, int current1D)
    {
        Coord lastCoord = currentCoord + (winLength - 1) * dir;
        if (!IsInsideBoard(lastCoord)) return false;

        int lastCoordIndex = Convert2DTo1D(lastCoord);
        if (caroState[current1D] != caroState[lastCoordIndex]) return false;

        for (int step = 1; step < winLength; step++)
        {
            Coord nextCoord = currentCoord + step * dir;
            int nextIndex = Convert2DTo1D(nextCoord);   

            if (caroState[nextIndex] != caroState[current1D]) return false;
        }
        return true;
    }

    private bool CheckDraw()
    {
        foreach(var state in caroState)
        {
            if(state is CaroState.Empty) return false;
        }
        return true;
    }
    //private bool CheckBotWin()
    //{

    //}
    private bool IsInsideBoard(Coord coord)
    {
        if (coord.row >= boardSize || coord.row < 0 || coord.col >= boardSize || coord.col < 0) return false;
        return true;
    }

    private int Convert2DTo1D(Coord coord)
    {
        return coord.row * boardSize + coord.col;
    }
    
}