using UnityEngine;

public static class SaveSystem
{
    // Guarda si un nivel está desbloqueado
    public static void SetNivelDesbloqueado(int nivel)
    {
        PlayerPrefs.SetInt("Nivel_" + nivel, 1);
        PlayerPrefs.Save();
    }

    // Revisa si un nivel está desbloqueado
    public static bool EstaDesbloqueado(int nivel)
    {
        return PlayerPrefs.GetInt("Nivel_" + nivel, nivel == 1 ? 1 : 0) == 1;
        // Nivel 1 está desbloqueado por defecto
    }

    // Guardar última escena jugada
    public static void GuardarUltimaEscena(string nombreEscena)
    {
        PlayerPrefs.SetString("UltimaEscena", nombreEscena);
        PlayerPrefs.Save();
    }

    public static string CargarUltimaEscena()
    {
        return PlayerPrefs.GetString("UltimaEscena", "Nivel1");
    }
}