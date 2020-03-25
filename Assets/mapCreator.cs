using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class mapCreator : MonoBehaviour
{
    public static mapCreator instance;

    [SerializeField] private Transform[] Audios;
    [SerializeField] private Transform[] Puzzles;
    [SerializeField] public roomClass[] Rooms;

    void Awake()
    {
        instance = this;
    }

    public void Initialize()
    {
        for (int i = 0; i < Puzzles.Length; i++)
        {
            Audios[i].position = Rooms[PlayerState.PuzzlePositions[i]].PuzzlePosition;
            Puzzles[i].position = Rooms[PlayerState.PuzzlePositions[i]].PuzzlePosition;

            Rooms[PlayerState.PuzzlePositions[i]].Puzzle = Puzzles[i].gameObject;

        }

    }


}
