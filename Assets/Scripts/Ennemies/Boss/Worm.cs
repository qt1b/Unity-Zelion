using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;

public class Worm : MonoBehaviourPunCallbacks
{

    private Animator _animator;
    private bool _isAttacking = false;
    private bool _right = false;
    private readonly float _range = 1f;

    private const float _moveTime = 1f;
    private const float _attackTime = 1f;
    
    private static readonly Vector3 displacement = new (0, 1);

    private static readonly int _attack = Animator.StringToHash("Attack");
    private static readonly int __right = Animator.StringToHash("Right");
    

    private readonly System.Random _random = new();
    // Start is called before the first frame update
    void Awake()
    {
        if (PhotonNetwork.OfflineMode || PhotonNetwork.IsMasterClient) return;
        enabled = false;
        _animator ??= GetComponent<Animator>();
        
        InvokeRepeating(nameof(Refresh), 0.1f, 0.1f);
    }


    IEnumerator Attack()
    {
        _isAttacking = true;
        transform.position = new Vector3(-60, 0);
        var list_players = GlobalVars.PlayerList.Where(g => g.GetComponent<Player.Player>().IsAlive()).ToArray();
        var player = list_players[_random.Next(list_players.Length)];
        _right = _random.Next(0, 1) == 0;
        yield return new WaitForSeconds(_moveTime);
        transform.position = player.transform.position + (_right ? -displacement : displacement);
        
        _animator.SetBool(_attack, true);
        _animator.SetBool(__right, _right);

        yield return new WaitForSeconds(_attackTime);
        GlobalVars.PlayerList.Where(g =>
            g.GetComponent<Player.Player>().IsAlive() && (transform.position - g.transform.position).magnitude<= _range);
        
        _animator.SetBool(_attack, false);
        _isAttacking = false;
    }
    

    void Refresh()
    {
        if (!_isAttacking)
            StartCoroutine(Attack());
    }
}
