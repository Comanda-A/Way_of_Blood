using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Цель (игрок)
    public Vector2 offset = new Vector2(0, 0); // Смещение камеры

    [Header("Follow Settings")]
    public float smoothSpeed = 0.125f; // Плавность слежения (как в оригинале)
    public float deadzoneRadius = 2f; // Радиус зоны, в которой камера не двигается

    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.5f; // Длительность тряски
    public float shakeMagnitude = 0.1f; // Сила тряски

    private Vector3 _originalPosition; // Текущая "штатная" позиция камеры (без тряски)
    private bool _isShaking = false;
    private float _shakeTimer = 0f;

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. Обновляем "штатную" позицию камеры (плавное следование за игроком)
        UpdateCameraFollow();

        // 2. Если есть тряска — добавляем к позиции случайное смещение
        if (_isShaking)
        {
            UpdateShake();
        }
    }

    /// <summary> Плавное следование за игроком (как в оригинале) </summary>
    private void UpdateCameraFollow()
    {
        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        // Если игрок вне deadzone — двигаем камеру
        if (Vector2.Distance(transform.position, desiredPosition) > deadzoneRadius)
        {
            _originalPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            _originalPosition = transform.position; // Сохраняем текущую позицию
        }

        // Применяем позицию (без тряски)
        transform.position = _originalPosition;
    }

    /// <summary> Запуск тряски камеры </summary>
    public void ShakeCamera()
    {
        if (_isShaking) return;
        _isShaking = true;
        _shakeTimer = shakeDuration;
    }

    /// <summary> Обновление тряски </summary>
    private void UpdateShake()
    {
        if (_shakeTimer > 0)
        {
            // Добавляем к штатной позиции случайное смещение
            transform.position = _originalPosition + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            // Завершаем тряску
            _isShaking = false;
            transform.position = _originalPosition;
        }
    }
}