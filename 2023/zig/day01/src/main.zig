const std = @import("std");

pub fn main() !void {
    // read input
    const lines = readInputFile("input.txt") catch |err| {
        std.debug.print("Error: {}\n", .{err});
        return err;
    };

    // process
    for (lines, 0..) |line, index| {
        _ = index;

        // process line
        std.debug.print("{s}", .{line});
    }

    // write output
}

pub fn readInputFile(comptime path: []const u8) ![][]const u8 {
    const file = try std.fs.cwd().openFile(path, .{});
    defer file.close();

    var arena = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    defer arena.deinit();
    const allocator = arena.allocator();

    const buffer_size = 4096;
    const file_buffer = try file.readToEndAlloc(allocator, buffer_size);
    defer allocator.free(file_buffer);

    var iter = std.mem.split(u8, file_buffer, "\n");

    var lines: [100][]const u8 = undefined;
    var index: u32 = 0;
    while (iter.next()) |line| : (index += 1) {
        std.debug.print("read line: {s}", .{line});
        lines[index] = line;
    }
    return lines[0..index];
}
