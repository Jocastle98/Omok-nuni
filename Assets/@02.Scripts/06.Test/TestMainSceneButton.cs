using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TestMainSceneButton : MonoBehaviour
{
    [SerializeField] private GameObject mOmoknuniPrefab;
    [SerializeField] private GameObject mStonePrefab;
    [SerializeField] private Transform[] mStoneTargets;
    [SerializeField] private Transform[] mFalling;
    [SerializeField] private Transform[] mStartPos;
    [SerializeField] private float flyDuration;
    [SerializeField] private float fallDuration;
    [SerializeField] private GameObject collisionEffectPrefab;
    [SerializeField] private GameObject stackEffectPrefab;

    public void StartClickAnimation(int buttonIndex, Action onClickAction)
    {
        int ranValue = Random.Range(0, mStartPos.Length);
        var omoknuniObject = Instantiate(mOmoknuniPrefab, mStartPos[ranValue].position, Quaternion.identity);

        omoknuniObject.transform.DOMove(mStoneTargets[buttonIndex].position, flyDuration).SetEase(Ease.InQuad)
            .OnComplete(
                () =>
                {
                    Instantiate(collisionEffectPrefab, omoknuniObject.transform.position, Quaternion.identity);
                    mStoneTargets[buttonIndex].gameObject.SetActive(false);
                    Vector3 spawnPos = mStoneTargets[buttonIndex].position;
                    var fallingStone = Instantiate(mStonePrefab, spawnPos, quaternion.identity);
                    fallingStone.transform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo);
                    fallingStone.transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360);
                    fallingStone.transform.DOMove(mFalling[buttonIndex].position, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
                    {
                        //Destroy(collisionEffectPrefab);
                        Instantiate(stackEffectPrefab, fallingStone.transform.position, Quaternion.identity);
                        DOVirtual.DelayedCall(3f, () =>
                        {
                            Destroy(fallingStone);
                            //Destroy(stackEffectPrefab);
                        });
                    });
                    
                    omoknuniObject.transform.DORotate(new Vector3(0, 0, 360), fallDuration, RotateMode.FastBeyond360);
                    omoknuniObject.transform.DOMoveY(mStoneTargets[buttonIndex].position.y - 10f, fallDuration)
                        .SetEase(Ease.InQuad).OnComplete(() => Destroy(omoknuniObject));

                    
                    DOVirtual.DelayedCall(1f, ()=> onClickAction?.Invoke());
                    mStoneTargets[buttonIndex].gameObject.SetActive(true);

                });
    }
}
