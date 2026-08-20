C#

using System;
using System.Reflection;

namespace GrandLarcenyAutoExploit
{
    class Program 
    {
        static void Main(string[] args) 
        {
            Console.WriteLine("[*] Iniciando exploit para Grand Larceny Auto (TryHackMe)...");

            try
            {
                // 1. Cargar el ensamblado .NET del juego
                string assemblyPath = "GrandLarcenyAuto.dll";
                var asm = Assembly.LoadFrom(assemblyPath);
                Console.WriteLine($"[+] Ensamblado '{assemblyPath}' cargado correctamente.");

                // 2. Obtener las referencias de las clases requeridas
                var playerStateType = asm.GetType("GrandLarcenyAuto.PlayerState");
                var vaultType = asm.GetType("GrandLarcenyAuto.SafehouseVault");

                if (playerStateType == null || vaultType == null)
                {
                    Console.WriteLine("[-] Error: No se encontraron los tipos 'PlayerState' o 'SafehouseVault' en la DLL.");
                    return;
                }

                // 3. Instanciar el objeto PlayerState y forzar WantedStars = 6
                var player = Activator.CreateInstance(playerStateType);
                var wantedStarsProp = playerStateType.GetProperty("WantedStars");

                if (wantedStarsProp == null || player == null)
                {
                    Console.WriteLine("[-] Error: No se pudo obtener la propiedad 'WantedStars' o instanciar el jugador.");
                    return;
                }

                // Forzamos exactamente 6 estrellas para que la clave de desencriptación ("stars=6") coincida
                wantedStarsProp.SetValue(player, 6);
                Console.WriteLine("[+] Variable 'WantedStars' establecida en EXACTAMENTE 6 estrellas.");

                // 4. Instanciar la bóveda pasando el objeto 'player' como parámetro de constructor
                var vault = Activator.CreateInstance(vaultType, player);
                var tryOpenMethod = vaultType.GetMethod("TryOpen");

                if (tryOpenMethod == null || vault == null)
                {
                    Console.WriteLine("[-] Error: No se pudo instanciar la bóveda o encontrar el método 'TryOpen'.");
                    return;
                }

                // 5. Invocar el método TryOpen() y obtener la salida desencriptada
                Console.WriteLine("[*] Invocando SafehouseVault.TryOpen()...\n");
                var result = tryOpenMethod.Invoke(vault, null);

                Console.WriteLine("================ RESULTADO OBTENIDO ================");
                Console.WriteLine(result);
                Console.WriteLine("====================================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Ocurrió un error durante la ejecución: {ex.Message}");
            }
        }
    }
}
