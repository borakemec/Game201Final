using Data.ValueObjects;
using Keys;
using Managers;
using Sirenix.OdinInspector;
using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random=UnityEngine.Random;

namespace Controllers.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private PlayerManager manager;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Collider collider;

        #endregion

        #region Private Variables

        [ShowInInspector] private MovementData _data;
        [ShowInInspector] private bool _isReadyToMove, _isReadyToPlay, _isOnMiniGame, _hasReachedTarget;
        [ShowInInspector] private float _xValue;
        float boostSpeed;
        public bool hasStopped;
        private float2 _clampValues;

        #endregion

        #endregion

        internal void GetMovementData(MovementData movementData)
        {
            _data = movementData;
        }

        void Start()
        {
            hasStopped = false;
            boostSpeed = Random.Range(9f, 124f);
        }

        private void FixedUpdate()
        {
            if (!_isReadyToPlay)
            {
                StopPlayer();
                return;
            }

            if (_isOnMiniGame)
            {
                MovePlayerMiniGame();
            }

            if (_isReadyToMove)
            {
                MovePlayer();
            }

            else if (!_isOnMiniGame && !_isReadyToMove)
            {
                StopPlayerHorizontaly();
            }
        }

        private void StopPlayerHorizontaly()
        {
            rigidbody.velocity = new float3(0, rigidbody.velocity.y, _data.ForwardSpeed);
            rigidbody.angularVelocity = float3.zero;
        }

        private void MovePlayer()
        {
            var velocity = rigidbody.velocity;
            velocity = new float3(_xValue * _data.SidewaysSpeed, velocity.y,
                _data.ForwardSpeed);
            rigidbody.velocity = velocity;

            float3 position;
            position = new float3(
                Mathf.Clamp(rigidbody.position.x, _clampValues.x,
                    _clampValues.y),
                (position = rigidbody.position).y,
                position.z);
            rigidbody.position = position;
        }
        
        public TextMeshProUGUI boostText;
        private void MovePlayerMiniGame()
        {
            var velocity = rigidbody.velocity;
            if(_hasReachedTarget)
            {
                _data.ForwardSpeed = Mathf.MoveTowards(_data.ForwardSpeed, 0f, Time.deltaTime * (boostSpeed/4));
            }
            else
            {
                _data.ForwardSpeed = boostSpeed;
            }
            velocity = new float3(_xValue * _data.SidewaysSpeed, velocity.y, _data.ForwardSpeed);
            rigidbody.velocity = velocity;

            float3 position;
            position = new float3(
                Mathf.Clamp(rigidbody.position.x, _clampValues.x, _clampValues.y),
                (position = rigidbody.position).y,
                position.z);
            rigidbody.position = position;

            if(_data.ForwardSpeed == boostSpeed)
            {
                boostText.text = "Boost " + ((int)boostSpeed).ToString();
                _hasReachedTarget = true;
                Invoke("DisableBoostText", 2f);
            }
            if(_data.ForwardSpeed == 0f)
            {
                _data.SidewaysSpeed = 0f;
                hasStopped = true;
            }
        }

        void DisableBoostText(){
            boostText.enabled = false;
            return;
        }

        private void StopPlayer()
        {
            rigidbody.velocity = float3.zero;
            rigidbody.angularVelocity = float3.zero;
        }

        internal void IsReadyToPlay(bool condition)
        {
            _isReadyToPlay = condition;
        }

        internal void IsReadyToMove(bool condition)
        {
            _isReadyToMove = condition;
        }

        internal void IsOnMiniGame(bool condition)
        {
            _isOnMiniGame = condition;
        }

        internal void UpdateInputParams(HorizontalInputParams inputParams)
        {
            _xValue = inputParams.HorizontalInputValue;
            _clampValues = new float2(inputParams.HorizontalInputClampNegativeSide,
                inputParams.HorizontalInputClampPositiveSide);
        }

        internal void OnReset()
        {
            StopPlayer();
            _isReadyToMove = false;
            _isReadyToPlay = false;
            _isOnMiniGame = false;
            _hasReachedTarget = false;
            hasStopped = false;
        }
    }
}