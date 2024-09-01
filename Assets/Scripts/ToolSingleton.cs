using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using static Rotate;
using static Tool;
using static GameController;

public class ToolSingleton : MonoBehaviour
{
    public static ToolSingleton Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator MoveToPosition(GameObject chess, Vector3 targetPosition, float speed = 0.1f)
    {
        Tween moveTween = chess.transform.DOMove(targetPosition, speed).SetEase(Ease.OutQuad);
        yield return moveTween.WaitForCompletion();
    }
    public IEnumerator AIMoveChess(GameObject chess)
    {
        Chess chessPiece = chess.GetComponent<Chess>();

        int steps = GetLatestDiceResult();
        for (int i = 0; i < steps; i++)
        {
            Space currentSpace = chessPiece.CheckSelfPos().GetComponent<Space>();

            if (currentSpace.next_space == null)
            {
                break;
            }

            Vector3 nextPosition;

            if (currentSpace.next_space == chessPiece.start_space)
            {
                if (chessPiece.CheckSelfPos().layer == 6)//Home
                {
                    nextPosition = chessPiece.start_space.GetComponent<Space>().actual_position;
                    yield return MoveToPosition(chess, nextPosition);
                    break;
                }
                else if (chessPiece.CheckSelfPos().layer == 8)//Path
                {
                    nextPosition = currentSpace.next_space2.GetComponent<Space>().actual_position;

                }
                else
                {
                    nextPosition = currentSpace.next_space.GetComponent<Space>().actual_position;
                }
            }
            else
            {
                nextPosition = currentSpace.next_space.GetComponent<Space>().actual_position;

                // Vector3 next_pos = currentSpace.next_space.GetComponent<Space>().actual_position;
                // chessPiece.transform.position = next_pos;
            }
            yield return MoveToPosition(chess, nextPosition);

            // 短暂暂停，让玩家能看清每一步的移动
            yield return new WaitForSeconds(0.1f);

        }
        isChessMoved = true;
    }
}
