package command

import (
	"bufio"
	"fmt"
	"os"
	"path/filepath"
	"strings"

	"github.com/mlouage/envtamer-go/internal/storage"
	"github.com/spf13/cobra"
)

func writeEnvFile(path string, envVars map[string]string) error {
	file, err := os.Create(path)
	if err != nil {
		return fmt.Errorf("failed to create file: %w", err)
	}
	defer file.Close()

	writer := bufio.NewWriter(file)
	for key, value := range envVars {
		if strings.ContainsAny(value, " \t\n\r") {
			// Quote values with whitespace
			_, err = fmt.Fprintf(writer, "%s=\"%s\"\n", key, value)
		} else {
			_, err = fmt.Fprintf(writer, "%s=%s\n", key, value)
		}
		if err != nil {
			return fmt.Errorf("failed to write to file: %w", err)
		}
	}

	if err := writer.Flush(); err != nil {
		return fmt.Errorf("failed to flush writer: %w", err)
	}

	return nil
}

func newPullCmd() *cobra.Command {
	var filename string

	cmd := &cobra.Command{
		Use:   "pull DIRECTORY_NAME",
		Short: "Pull environment variables from the database to a local .env file",
		Long:  `This command retrieves stored environment variables for the specified directory from the database and writes them to a local .env file. If the file already exists, it will prompt for confirmation before overwriting.`,
		Args:  cobra.ExactArgs(1),
		RunE: func(cmd *cobra.Command, args []string) error {
			// Resolve directory path
			dirPath, err := resolvePath(args[0])
			if err != nil {
				return fmt.Errorf("failed to resolve directory path: %w", err)
			}

			// Get environment variables from database
			db, err := storage.New()
			if err != nil {
				return fmt.Errorf("failed to create storage: %w", err)
			}
			defer db.Close()

			envVars, err := db.GetEnvVars(dirPath)
			if err != nil {
				return fmt.Errorf("failed to get env vars: %w", err)
			}

			// Check if file exists
			envFilePath := filepath.Join(".", filename)
			if _, err := os.Stat(envFilePath); err == nil {
				fmt.Printf("File '%s' already exists. Overwrite? (y/N): ", envFilePath)
				var response string
				_, err := fmt.Scanln(&response)
				if err != nil {
					return err
				}
				if !strings.HasPrefix(strings.ToLower(response), "y") {
					fmt.Println("Operation cancelled.")
					return nil
				}
			}

			if err := writeEnvFile(envFilePath, envVars); err != nil {
				return fmt.Errorf("failed to write env file: %w", err)
			}

			fmt.Printf("Successfully pulled %d environment variables to file: %s\n", len(envVars), envFilePath)
			return nil
		},
	}

	cmd.Flags().StringVarP(&filename, "filename", "f", ".env", "The name of the env file to create or update")
	return cmd
}
