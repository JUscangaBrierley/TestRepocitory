package com.brierley.loyaltyware.clientlib;

import java.io.*;

public class LWLogger {
	private Boolean _toConsole = false;
	// private FileWriter _writer = null;
	private BufferedWriter _writer = null;

	public LWLogger(Boolean toConsole, String outputFile) {
		_toConsole = toConsole;
		if (!LWUtils.isEmptyString(outputFile)) {
			File f = new File(outputFile);
			if (!f.exists()) {
				// create the file.
				try {
					f.createNewFile();
				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			} else if (!f.canWrite()) {
				// what to do here?
			}
			try {
				FileWriter writer = new FileWriter(outputFile, true);
				_writer = new BufferedWriter(writer);
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}

	private void writeToConsole(String message) {
		if (_toConsole) {
			System.out.println(message);
		}
	}

	private void writeToConsole(Exception ex) {
		System.out.println(ex.getMessage());
		ex.printStackTrace(System.out);
	}

	private void writeToFile(String message) {
		if (_writer != null) {
			try {
				_writer.write(message);
				_writer.newLine();

			} catch (Exception ex) {
				writeToConsole("ERROR Writing to file.");
				writeToConsole(ex);
			} finally {
				if (_writer != null) {
					try {
						_writer.flush();
					} catch (IOException e) {
						e.printStackTrace();
					}
				}
			}
		}
	}

	public void writeMessage(String message) {
		writeToConsole(message);
		writeToFile(message);
	}
}
