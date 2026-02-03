using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Magic Spell", menuName = "Magic System/Magic Spell Data")]
public class MagicSpellData : ScriptableObject
{
    [Header("Spell Info")]
    public string spellName = "Magia 1";
    
    [Header("QTE Configuration")]
    [Tooltip("Lista de botones y sus posiciones en la línea del QTE")]
    public List<QTEButtonData> buttons = new List<QTEButtonData>();
    
    [Header("QTE Audio")]
    [Tooltip("Clip de audio que se reproduce cuando el QTE empieza a moverse")]
    public AudioClip qteAudioClip;
    
    [Header("Damage Area")]
    [Tooltip("Define el área de daño y daño base de esta magia")]
    public MagicDamageArea damageArea;
    
    [Header("Spell Effects")]
    [Tooltip("Multiplicador del cooldown de la magia")]
    public float cooldownMultiplier = 1f;
    
    [Header("Visual Effects")]
    [Tooltip("Prefab de VFX que se muestra al ejecutar la magia")]
    public GameObject vfxPrefab;
    
    [Tooltip("Offset de posición del VFX relativo al jugador")]
    public Vector2 vfxOffset = Vector2.zero;
    
    [Tooltip("Duración del VFX antes de destruirse (0 = usar Animator para auto-destrucción)")]
    public float vfxDuration = 2f;
}

[System.Serializable]
public class QTEButtonData
{
    [Tooltip("Tecla de flecha a presionar (UpArrow, DownArrow, LeftArrow, RightArrow)")]
    public KeyCode key = KeyCode.UpArrow;
    
    [Tooltip("Posición en la línea del QTE (0.0 = inicio, 1.0 = final)")]
    [Range(0f, 1f)]
    public float position = 0.5f;
}
