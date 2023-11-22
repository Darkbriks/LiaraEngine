using System.IO;

namespace LiaraEditor.Utilities
{
    /// <summary>
    /// Cette classe contient des méthodes pour valider les projets Liara et leurs composants, comme le nom du projet ou le chemin du projet
    /// </summary>
    public static class ProjectValidator
    {
        /// <summary/>
        /// Enumération des codes d'erreur possibles lors de la validation du nom d'un projet Liara
        /// </summary>
        public enum ProjectNameError
        {
            // Pas d'erreur
            None = 0,

            // Le nom est null, vide ou composé uniquement d'espaces
            NullOrWhiteSpace = 1,

            // Le nom contient des caractères interdits par Windows
            ForbiddenWindowsCharacters = 2,

            // Le nom contient des caractères interdits par Liara
            ForbiddenLiaraCharacters = 3,

            // Le nom est réservé par Windows
            ReservedWindowsName = 4,

            // Le nom est réservé par Liara
            ReservedLiaraName = 5,

            // Le nom est déjà utilisé par un fichier ou un dossier
            AlreadyUsedName = 6,

            // Le nom contient des caractères de contrôle
            ControlCharacters = 7,

            // Erreur non spécifiée
            UnspecifiedError = 8
        }

        /// <summary/>
        /// Enumération des codes d'erreur possibles lors de la validation du chemin d'un projet Liara
        /// </summary>
        public enum ProjectPathError
        {
            // Pas d'erreur
            None = 0,

            // Le chemin est réservé par Windows
            ReservedWindowsPath = 1,

            // Le chemin est réservé par Liara
            ReservedLiaraPath = 2,

            // Le chemin n'existe pas
            PathDoesNotExist = 3,

            // Le chemin n'est pas un dossier
            PathIsNotADirectory = 4,

            // Le chemin n'est pas accessible en lecture, écriture, éxécution ou création
            PathIsNotAccessible = 5,

            // Erreur non spécifiée
            UnspecifiedError = 6
        }

        /// <summary>
        /// Liste des caractères invalides pour le nom d'un projet Liara
        /// </summary>
        private static readonly char[] liaraInvalidCharacters = { ' ', '.', ',', ';', ':', '/', '\\', '|', '<', '>', '?', '*', '"', '\'', '[', ']', '{', '}', '(', ')', '!', '@', '#', '$', '%', '^', '&', '*', '~', '`', '+', '=', '-' };

        /// <summary>
        /// Liste des mots réservés pour le nom d'un projet Liara
        /// </summary>
        private static readonly string[] liaraReservedNames = { "Liara", "Editor", "Template", "Engine", // Expression pouvant être utilisée par Liara qui doivent donc être réservées
                                                                       "Null", "Void", "None", "Empty", "Default" }; // Expression pouvant être interprétée comme des valeurs par Liara qui doivent donc être réservées

        /// <summary>
        /// Liste des chemins réservés pour le chemin d'un projet Liara
        /// </summary>
        private static readonly string[] liaraReservedPaths = { "Liara", "Editor", "Template", "Engine" }; // Expression pouvant être utilisée par des dossiers Liara qui doivent donc être réservées


        /// <summary>
        /// Cette méthode vérifie si le nom du projet contient des caractères invalides
        /// <list type="table">
        ///     Les caractères invalides sont :
        ///     <list type="bullet">
        ///         <item>Les caractères interdits par Windows pour les noms de fichiers (voir <see cref="Path.GetInvalidFileNameChars"/>) ou de dossiers (voir <see cref="Path.GetInvalidPathChars"/>)</item>
        ///         <item>Les caractères interdits par Liara (voir <see cref="liaraInvalidCharacters"/>)</item>
        ///         <item>Les caractères de contrôle (voir <see cref="char.IsControl(char)"/>)</item>
        ///     </list>
        /// </list>
        /// Dans tous les cas, la comparaison est insensible à la casse
        /// La méthode retourne l'erreur trouvée, et, le cas échéant, le caractère invalide trouvé, ou null si aucun erreur n'a été trouvée
        /// </summary>
        private static (ProjectNameError, string) ContainsInvalidCharacter(string projectName)
        {
            // Vérifier les caractères invalides pour les noms de fichiers
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            foreach (char invalidChar in invalidFileNameChars)
            {
                if (projectName.IndexOf(invalidChar, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return (ProjectNameError.ForbiddenWindowsCharacters, invalidChar.ToString());
                }
            }

            // Vérifier les caractères invalides pour les noms de dossiers
            char[] invalidPathChars = Path.GetInvalidPathChars();
            foreach (char invalidChar in invalidPathChars)
            {
                if (projectName.IndexOf(invalidChar, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return (ProjectNameError.ForbiddenWindowsCharacters, invalidChar.ToString());
                }
            }

            // Vérifier les caractères invalides pour Liara
            foreach (char invalidChar in liaraInvalidCharacters)
            {
                if (projectName.IndexOf(invalidChar, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return (ProjectNameError.ForbiddenLiaraCharacters, invalidChar.ToString());
                }
            }

            // Vérifier les caractères de contrôle
            foreach (char invalidChar in projectName)
            {
                if (char.IsControl(invalidChar))
                {
                    return (ProjectNameError.ControlCharacters, invalidChar.ToString());
                }
            }

            return (ProjectNameError.None, null);
        }

        /// <summary>
        /// Cette méthode vérifie si le nom du projet contient un nom réservé
        /// <list type="table">
        /// Les noms réservés sont :
        ///     <list type="bullet">
        ///     <item>Les noms réservés par Liara (voir <see cref="liaraReservedNames"/>)</item>
        ///     <item>Les noms de fichiers ou de dossiers existants</item>
        ///     </list>
        /// </list>
        /// Dans tous les cas, la comparaison est insensible à la casse
        /// La méthode retourne l'erreur trouvée, et, le cas échéant, le nom réservé trouvé, ou null si aucun erreur n'a été trouvée
        /// </summary>
        private static (ProjectNameError, string) ContainsReservedName(string projectName, string projectPath)
        {
            // Vérifier si les noms réservés par Windows
            // TODO: Vérifier les noms réservés par Windows

            // Vérifier les noms réservés par Liara
            foreach (string invalidName in liaraReservedNames)
            {
                if (projectName.Contains(invalidName, StringComparison.OrdinalIgnoreCase))
                {
                    return (ProjectNameError.ReservedLiaraName, invalidName);
                }
            }

            // Vérifier si le nom du projet est déjà utilisé par un fichier ou un dossier
            if (File.Exists(projectPath + projectName) || Directory.Exists(projectPath + projectName))
            {
                return (ProjectNameError.AlreadyUsedName, projectName);
            }

            return (ProjectNameError.None, null);
        }

        /// <summary>
        /// Cette méthode vérifie si le nom du projet est valide
        /// <list type="bullet">
        ///     Pour être valide, le nom ne doit présenter aucune erreur (voir <see cref="ProjectNameError"/>)
        /// </list>
        /// Dans tous les cas, la comparaison est insensible à la casse
        /// La méthode retourne l'erreur trouvée, et, le cas échéant, le caractère invalide ou le nom réservé trouvé, ou null si aucun erreur n'a été trouvée
        /// </summary>
        public static (ProjectNameError, string) ValidateProjectName(string projectName, string projectPath)
        {
            // Code 1
            if (string.IsNullOrWhiteSpace(projectName))
            {
                return (ProjectNameError.NullOrWhiteSpace, null);
            }

            // Code 2, 3, 7
            (ProjectNameError, string)? invalidCharacter = ContainsInvalidCharacter(projectName);
            if (invalidCharacter?.Item1 != ProjectNameError.None)
            {
                return invalidCharacter.Value;
            }

            // Code 4, 5, 6
            (ProjectNameError, string)? reservedName = ContainsReservedName(projectName, projectPath);
            if (reservedName?.Item1 != ProjectNameError.None)
            {
                return reservedName.Value;
            }

            // Code 0
            return (ProjectNameError.None, null);
        }

        /// <summary>
        /// Cette méthode vérifie si le chemin du projet contient un nom réservé
        /// <list type="table">
        /// Les noms réservés sont :
        ///     <list type="bullet">
        ///     Les noms réservés par Liara (voir <see cref="liaraReservedPaths"/>)
        ///     </list>
        /// </list>
        /// Dans tous les cas, la comparaison est insensible à la casse
        /// La méthode retourne l'erreur trouvée, et, le cas échéant, le nom réservé trouvé, ou null si aucun erreur n'a été trouvée
        /// </summary>
        private static (ProjectPathError, string) ContainsReservedPath(string projectPath)
        {
            // Vérifier les noms réservés par Windows
            // TODO: Vérifier les noms réservés par Windows

            // Vérifier les noms réservés par Liara
            foreach (string invalidName in liaraReservedPaths)
            {
                if (projectPath.Contains(invalidName, StringComparison.OrdinalIgnoreCase))
                {
                    return (ProjectPathError.ReservedLiaraPath, invalidName);
                }
            }

            return (ProjectPathError.None, null);
        }

        /// <summary>
        /// Cette méthode vérifie si le chemin du projet est valide
        /// 
        /// <list type="bullet">
        ///     Pour être valide, le chemin ne doit présenter aucune erreur (voir <see cref="ProjectPathError"/>)
        /// </list>
        /// Dans tous les cas, la comparaison est insensible à la casse
        /// La méthode retourne l'erreur trouvée, et, le cas échéant, le nom réservé trouvé, ou null si aucun erreur n'a été trouvée
        ///     
        /// <para>/!\ Le nom du projet ne doit pas être inclus dans le chemin du projet</para>
        /// <para>Exemple : Le chemin "C:\Liara Projects\MyProject\MyProject" est invalide car il contient le nom du projet "MyProject" et pourrait donc renvoyer un code d'erreur qui n'a pas de sens</para>
        /// </summary>
        public static (ProjectPathError, string) ValidateProjectPath(string projectPath)
        {
            // Code 1, 2
            if (string.IsNullOrWhiteSpace(projectPath))
            {
                return (ProjectPathError.PathDoesNotExist, null);
            }

            // Code 3
            if (!Directory.Exists(projectPath))
            {
                return (ProjectPathError.PathDoesNotExist, null);
            }

            // Code 4
            if (File.Exists(projectPath))
            {
                return (ProjectPathError.PathIsNotADirectory, null);
            }

            // Code 5
            try
            {
                string testFilePath = Path.Combine(projectPath, "LiaraTestFile.txt");
                File.WriteAllText(testFilePath, "Ce fichier est utilisé pour tester l'accès en écriture du dossier dans lequel vous souhaitez créer votre projet Liara\n" +
                    "Ce fichier est normalement supprimé automatiquement après la création du projet\nSi ce n'est pas le cas, vous pouvez le supprimer manuellement");
                File.Delete(testFilePath);
            }
            catch (UnauthorizedAccessException)
            {
                return (ProjectPathError.PathIsNotAccessible, null);
            }
            catch (Exception ex)
            {
                return (ProjectPathError.UnspecifiedError, ex.Message);
            }

            return (ProjectPathError.None, null);
        }
    }
}
