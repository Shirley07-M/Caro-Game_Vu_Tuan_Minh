using System;
using System.Collections.Generic;
public static class CaroAI //* Cắt đứt liên hệ với unity để chạy multi thread (Nếu dính tới unity thường bị ép chạy main thread)
{
    private static readonly Random random = new();
    public static int RandomMove(CaroState[] boardState)
    {
        List<int> emptyCaroIndex = new();
        for(int i = 0; i < boardState.Length; i++)
        {
            if (boardState[i] is CaroState.Empty) emptyCaroIndex.Add(i);
        }

        int randomIndex = random.Next(0, emptyCaroIndex.Count);

        return emptyCaroIndex[randomIndex];

    }
}