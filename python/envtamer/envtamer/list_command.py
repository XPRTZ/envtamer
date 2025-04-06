from envtamer_db.envtamer_db import EnvTamerDb
from envtamer.table_formatter import print_env_table

def list_command(directory: str):
    try:
        db = EnvTamerDb()

        if directory is None:
            db_directories = db.get_all_directories()

            for db_directory in db_directories:
                print(db_directory)
        else:
            env_vars = db.get_env_values(directory)
            if env_vars is None or len(env_vars) == 0:
                print(f'🛑 No Environment variables found for directory: {directory}')
            print_env_table(env_vars)
    except Exception as ex:
        print(f'🛑 pull encountered an exception: {ex}')