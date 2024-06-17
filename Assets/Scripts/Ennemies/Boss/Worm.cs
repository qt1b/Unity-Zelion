using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Global;
using JetBrains.Annotations;
using Photon.PhotonUnityNetworking.Code;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Worm : MonoBehaviourPunCallbacks
{

    public ushort damage = 5;

    private Animator _animator;
    private bool _isAttacking = false;
    private bool _right = false;
    private readonly float _range = 3f;

    private const float _moveTime = 1.5f;
    private const float _attackTime = 5/6f;
    
    private static readonly Vector3 displacement = new (1, -1f);

    private static readonly int _attack = Animator.StringToHash("Attack");
    private static readonly int __right = Animator.StringToHash("Right");
    

     private readonly System.Random _random = new System.Random();
    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
        {
            enabled = false;
            _animator ??= GetComponent<Animator>();
        }
        else
        {
            InvokeRepeating(nameof(Refresh), 0.1f, 0.1f);
        }
    }


    IEnumerator Attack()
    {
        _isAttacking = true;
        transform.position = new Vector3(-60, 0);
        var list_players = GlobalVars.PlayerList.Where(g => g.GetComponent<Player.Player>().IsAlive()).ToArray();
        var player = list_players[_random.Next(list_players.Length)];
        _right = _random.Next(0, 256) > 128;
        yield return new WaitForSeconds(_moveTime);
        transform.position = player.transform.position + new Vector3(_right ? -displacement.x : displacement.x, displacement.y);
        
        _animator.SetBool(_attack, true);
        _animator.SetBool(__right, _right);

        yield return new WaitForSeconds(_attackTime);
        foreach (var player1 in GlobalVars.PlayerList.Where(g =>
                     g.GetComponent<Player.Player>().IsAlive() && (transform.position - g.transform.position).magnitude<= _range))
        {
            Debug.Log("damaged");
           player1.TakeDamages(damage); 
        }

        yield return new WaitForSeconds(2 / 3f);
        
        _animator.SetBool(_attack, false);
        _isAttacking = false;
    }
    

    void Refresh()
    {
        if (!_isAttacking)
            StartCoroutine(Attack());
    }
}
