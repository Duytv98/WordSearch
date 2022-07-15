using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
public class SmallLeaderboard : MonoBehaviour
{


    [SerializeField] private LeaderboardController leaderboardController = null;
    private List<PlayerLB> _data;
    public EnhancedScroller hScroller;



    public EnhancedScroller.TweenType hScrollerTweenType = EnhancedScroller.TweenType.immediate;
    public float hScrollerTweenTime = 0f;
}
