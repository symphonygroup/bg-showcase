package com.example.demo.memo;

import javax.persistence.*;
import java.util.Objects;

@Entity
public class Memo {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    public Long id;
    public String owner;
    public String data;

    public Long getId() { return id; }
    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        Memo memo = (Memo) o;
        return Objects.equals(id, memo.id) && Objects.equals(owner, memo.owner) && Objects.equals(data, memo.data);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id, owner, data);
    }

    @Override
    public String toString() {
        return "Memo{" +
                "id=" + id +
                ", owner='" + owner + '\'' +
                ", data='" + data + '\'' +
                '}';
    }
}
