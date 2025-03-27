using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class MainButtonAnimation : MonoBehaviour
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
    [SerializeField] private GameObject mCloudObject;

    private void Start()
    {
        var cloudStartPos = mCloudObject.transform.position; 
        mCloudObject.transform.DOMoveY(cloudStartPos.y + 0.05f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void StartClickAnimation(int buttonIndex, Action onClickAction)
    {
        int ranValue = Random.Range(0, mStartPos.Length);
        var omoknuniObject = Instantiate(mOmoknuniPrefab, mStartPos[ranValue].position, Quaternion.identity);

        omoknuniObject.transform.DOMove(mStoneTargets[buttonIndex].position, flyDuration).SetEase(Ease.InQuad)
            .OnComplete(
                () =>
                {
                    AudioManager.Instance.PlaySfxSound(3);
                    omoknuniObject.GetComponent<Animator>().SetBool("isHit", true);
                    Instantiate(collisionEffectPrefab, omoknuniObject.transform.position, Quaternion.identity);
                    mStoneTargets[buttonIndex].gameObject.SetActive(false);
                    Vector3 spawnPos = mStoneTargets[buttonIndex].position;
                    var fallingStone = Instantiate(mStonePrefab, spawnPos, quaternion.identity);
                    fallingStone.transform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo);
                    fallingStone.transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360);
                    fallingStone.transform.DOMove(mFalling[buttonIndex].position, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
                    {
                        AudioManager.Instance.PlaySfxSound(2);
                        Instantiate(stackEffectPrefab, fallingStone.transform.position, Quaternion.identity);
                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            Destroy(fallingStone);
                        });
                    });

                    var randomDirection =
                        new Vector3(Random.Range(-5f, 5f), Random.Range(-10f, 10f), Random.Range(-5f, 5f));
                    
                    omoknuniObject.transform.DORotate(new Vector3(0, 0, 360), fallDuration, RotateMode.FastBeyond360);
                    omoknuniObject.transform.DOMove(mStoneTargets[buttonIndex].position + randomDirection, fallDuration)
                        .SetEase(Ease.OutQuad).OnComplete(() =>
                        {
                            Destroy(omoknuniObject);
                            mStoneTargets[buttonIndex].gameObject.SetActive(true);
                        });

                    
                    DOVirtual.DelayedCall(1f, ()=> onClickAction?.Invoke());
 
                });
    }
}
