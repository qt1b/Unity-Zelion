using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Ennemy : MonoBehaviour
{
    private bool _isShooter;
    private bool _isMelee;

    private ShootArrow _shooter;
    private MeleeAttack _meleeAttack;
    
    public float Speed;

    private GameObject _player;
    
    public float shootFrequency;
    public float meleeFrequency;

    public float shootRangeMax;
    public float shootRangeMin;
    public float meleeRange;

    private float _remainingShootingTime;
    private float _remainingMeleeTime;
    
    // Start is called before the first frame update
    void Start()
    {
        _shooter = GetComponent<ShootArrow>();
        _isShooter = _shooter is not null;
        
        _meleeAttack = GetComponent<MeleeAttack>();
        _isMelee = _meleeAttack is not null;
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        var playerPos = _player.transform.position;
        var direction = (pos - playerPos);
        var distance = Mathf.Abs(direction.magnitude);
        //print(distance);
        if (_isShooter && distance >= shootRangeMin && distance <= shootRangeMax)
        {
            if (_remainingShootingTime <= 0)
            {
                _remainingShootingTime = shootFrequency;
                _shooter.Shoot(0, pos, direction.normalized);
            }
            else
                _remainingShootingTime -= Time.deltaTime;
        }

        if (_isMelee && distance <= meleeRange)
        {
            if (_remainingMeleeTime <= 0)
            {
                _remainingMeleeTime = meleeFrequency;
                _meleeAttack.Attack();
            }
            else
                _remainingMeleeTime -= Time.deltaTime;
        }
    }
}
