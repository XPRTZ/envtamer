import os

from envtamer_db.envtamer_db import EnvTamerDb
from envtamer.file_handler import FileHandler

def pull_command(directory: str, path: str):
    try:
        if directory is None:
            directory = os.getcwd()

        db = EnvTamerDb()
        env_vars = db.get_env_values(directory)
        env_vars_dict = {env_var.Key : env_var.Value for env_var in env_vars}
        fh = FileHandler(directory)
        fh.write_env_file(path, env_vars_dict)
        print(f'✅ Pull successful: {directory}, {path}')
    except Exception as ex:
        print(f'🛑 pull encountered an exception: {ex}')